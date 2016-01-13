Public Class Mouvements


#Region "Déclaration variables, Structures, Enumérations"
    Private Const _InitialiseEchiquier = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" 'initialise la partie

    Private _JeuDePieces As String = "RNBQK" 'Jeu de pièces par défaut
    Dim Echiquier10x10(100) As Char 'contiendra l'échiquier complet (bords, cases vides, pièces)
    Private regles As New ReglesApplicables 'contiendra les règles à un moment donné de la partie
    Private ReglesRassemblees As New LectureFacileRegles 'servira à séparer les règles du scan du plateau de jeu(où sont les pièces etc..)

    'permettra l'ajout à l'échiquier de toutes les règles applicables dans une situation donnée
    Private Structure ReglesApplicables
        Dim Trait As CouleurDeLaPiece              'détermine à qui est le trait
        Dim peutFairePetitRoqueB As Boolean     'différents types de roques pour les 2 couleurs
        Dim peutFaireGrandRoqueB As Boolean
        Dim peutFairePetitRoqueN As Boolean
        Dim peutFaireGrandRoqueN As Boolean
        Dim numCaseEnPassant As Byte            'case où une prise en passant est possible(si cette case existe)
        Dim nbDepuisDernierPion As UInteger     'nb de coups depuis dernière prise/dernier coup de pion
        Dim nbCoupsJoues As Single       'nb de coups joués(on avance par demi-coup)
    End Structure
    'sert à déterminer la couleur d'une pièce sur une case
    Private Enum CouleurDeLaPiece 'suffisamment explicite
        Vide = 0
        Blanc = 1
        Noir = 2
        Bord = 3
        HorsEchiquier = 4
    End Enum
    'placer toutes les règles dans une structure pour une lecture plus simple
    Private Structure LectureFacileRegles
        Dim c1Pieces As String
        Dim c2AuTrait As String
        Dim c3DroitRoque As String
        Dim c4CaseEnPassant As String
        Dim c5nbDemiCoup As String
        Dim c6nbMouvement As String
    End Structure
#End Region

    Private Function ScanEchiquier(ByVal string_echiquier_complet As String) As Byte
        Dim lignes() As String
        Dim NumLigne As Integer
        Dim NumCol As Integer
        Dim LaLigne As String
        Dim listePiecesEtRegles() As String

        listePiecesEtRegles = string_echiquier_complet.Split(" ")


        'on place des bords partout
        For NumLigne = 0 To 99
            Echiquier10x10(NumLigne) = "*"
        Next

        lignes = listePiecesEtRegles(0).Split("/")

        If lignes.Count <> 8 Then
            Return 10
        End If

        For NumLigne = 0 To 7

            LaLigne = lignes(NumLigne)

            LaLigne = LaLigne.Replace("1", " ")
            LaLigne = LaLigne.Replace("2", "  ")
            LaLigne = LaLigne.Replace("3", "   ")
            LaLigne = LaLigne.Replace("4", "    ")
            LaLigne = LaLigne.Replace("5", "     ")
            LaLigne = LaLigne.Replace("6", "      ")
            LaLigne = LaLigne.Replace("7", "       ")
            LaLigne = LaLigne.Replace("8", "        ")

            If LaLigne.Length = 8 Then
                For NumCol = 0 To 7
                    Echiquier10x10((8 - NumLigne) * 10 + (NumCol + 1)) = LaLigne.Substring(NumCol, 1)
                Next
            Else
                Return 11 + NumLigne
            End If

        Next



        Return 0
    End Function

    Private Function SquareIndex(ByVal sqName As String) As Byte
        Dim lettre As Char
        Dim colonne As Byte
        Dim ligne As Byte

        If sqName.Length <> 2 Then Return 0

        lettre = sqName.Substring(0, 1) 'recupere la lettre
        colonne = Asc(lettre) - 96 'recupere le numero de colonne

        If colonne < 1 Or colonne > 8 Then Return 0

        ligne = sqName.Substring(1, 1) 'recupere le numero de ligne

        If ligne < 1 Or ligne > 8 Then Return 0

        Return ligne * 10 + colonne

    End Function
    'initialise l'échiquier quand new est appelé

#Region "Getter&Setter&Constructeur de la classe Mouvements"
    Sub New(Optional ByVal InitEchiquier As String = _InitialiseEchiquier) 'reçoit la constante en paramètre
        SetPosition(InitEchiquier)
    End Sub
    'récupère tout l'échiquier et les infos pratiques concernant les règles (Qui doit jouer, peut-on roquer? , peut-on faire la prise en passant ?,....)
    Public Function GetPosition() As String

        Dim colonne As Int16
        Dim ligne As Int16
        Dim numcase As Byte 'numéro de la case dans l'échiquer 10x10
        Dim string_echiquier_complet As String = "" 'contient tout l'échiquier (bord, cases vides, pièces) sous forme de string
        Dim nb_espace As Byte = 0 'compte le nombre d'espaces entre 2 occurences d'une pièce, permet de scanner l'échiquier et de savoir où se toruvent les pièces

        'fonction de balayage de l'échiquier, permet de parcourir l'échiquier ligne par ligne
        For ligne = 8 To 1 Step -1
            For colonne = 1 To 8
                numcase = ligne * 10 + colonne 'les cases étant numérotées de 0 à 99 dans un tableau de caractères, chaque ligne correspond à dix cases donc ligne 0 => 0 à 9, ligne 1 => 10 à 19, etc ....
                If Echiquier10x10(numcase) <> " " Then 'si un caractère est différent de espace, ça veut dire qu'il y a une pièce
                    If nb_espace > 0 Then               'donc si il y avait des espaces avant, on les rajoute au tableau et on remet le nombre d'espaces à 0, sinon on passe à la suite
                        string_echiquier_complet = string_echiquier_complet + nb_espace.ToString
                        nb_espace = 0
                    End If
                    string_echiquier_complet = string_echiquier_complet + Echiquier10x10(numcase) 'ensuite on ajoute la pièce à l'emplacement où elle se trouve sur l'échiquier(dans le tableau donc), puisqu'on vient de rajouter le nombre d'espaces adéquats entre cette pièce et la précédente
                Else
                    nb_espace = nb_espace + 1  'Le caractère rencontré est un espace, on ajoute un au nombre d'espaces. On fait cela jusqu'à la prochaine pièce rencontrée
                End If

            Next
            If nb_espace > 0 Then 'on s'apprête à changer de ligne, on enregistre le nombre d'espaces et on remet à 0 pour mettre le / symbolisant le bord au bon endroit
                string_echiquier_complet = string_echiquier_complet + nb_espace.ToString
                nb_espace = 0
            End If
            nb_espace = 0
            If ligne <> 1 Then string_echiquier_complet = string_echiquier_complet + "/" 'tant qu'on arrive pas au bout de l'échiquier (ligne =1), on met un slash à la fin de la ligne pour symboliser le bord
        Next

        'rajoute tout ce qui concerne les règles du jeu
        With regles
            string_echiquier_complet &= " " ' a&= "b" est l'équivalent de a = a & "b" donc là on est en train de rajouter un espace pour séparer les différentes règles ajoutées entre elles ainsi que des cases
            string_echiquier_complet &= IIf(.Trait = CouleurDeLaPiece.Blanc, "blanc", "noir") ' on ajoute à qui est le trait
            string_echiquier_complet &= " "
            string_echiquier_complet &= IIf(.peutFairePetitRoqueB, "K", "") 'on ajoute les règles concernant le roque pour les 2 couleurs
            string_echiquier_complet &= IIf(.peutFaireGrandRoqueB, "Q", "")
            string_echiquier_complet &= IIf(.peutFairePetitRoqueN, "k", "")
            string_echiquier_complet &= IIf(.peutFaireGrandRoqueN, "q", "")
            string_echiquier_complet &= IIf(.peutFaireGrandRoqueN Or .peutFairePetitRoqueN _
                            Or .peutFaireGrandRoqueB Or .peutFairePetitRoqueB, "", "-")
            string_echiquier_complet &= " "
            string_echiquier_complet &= IIf(regles.numCaseEnPassant <> 0, SquareIndex(regles.numCaseEnPassant), "-") 'on ajoute si oui on non une prise en passant est possible (et où ?)
            string_echiquier_complet &= " "
            string_echiquier_complet &= regles.nbDepuisDernierPion 'on ajoute combien de coups on été joués depuis le dernier mouvement de pion ou la derrnière prise(après 50 coups sans prise ni mouvement de pion, la partie est déclarée nulle)
            string_echiquier_complet &= " "
            string_echiquier_complet &= Math.Truncate(regles.nbCoupsJoues) 'on ajoute le nombre de coups joués.
        End With

        Return string_echiquier_complet 'renvoie le scan complet de l'échiquier + les règles

    End Function
    Public Function SetPosition(ByVal string_echiquier_complet As String) As Byte
        Dim situationRegles() As String

        situationRegles = string_echiquier_complet.Split(" ")

        'Si on a pas 6 infos sur les règles le string de l'échiquier est erroné
        If situationRegles.Count <> 6 Then
            Return 1
        End If

        With ReglesRassemblees
            .c1Pieces = situationRegles(0)
            .c2AuTrait = situationRegles(1)
            .c3DroitRoque = situationRegles(2)
            .c4CaseEnPassant = situationRegles(3)
            .c5nbDemiCoup = situationRegles(4)
            .c6nbMouvement = situationRegles(5)

            'détermination du trait
            If .c2AuTrait = "blanc" Then
                regles.Trait = CouleurDeLaPiece.Blanc
            ElseIf .c2AuTrait = "noir" Then
                regles.Trait = CouleurDeLaPiece.Noir
            Else
                Return 2
            End If

            'dertemination des droits au roque
            regles.peutFairePetitRoqueB = .c3DroitRoque.Contains("K")
            regles.peutFaireGrandRoqueB = .c3DroitRoque.Contains("Q")
            regles.peutFairePetitRoqueN = .c3DroitRoque.Contains("k")
            regles.peutFaireGrandRoqueN = .c3DroitRoque.Contains("q")

            'dertermination de la case e.p.
            If .c4CaseEnPassant <> "-" Then
                regles.numCaseEnPassant = SquareIndex(.c4CaseEnPassant)
            Else
                regles.numCaseEnPassant = 0
            End If

            'determnation du nombre de 1/2 coup
            regles.nbDepuisDernierPion = CInt(.c5nbDemiCoup)

            'determination du nombre de coup joué
            regles.nbCoupsJoues = CInt(.c6nbMouvement) + IIf(regles.Trait = CouleurDeLaPiece.Noir, 0.5, 0)


        End With

        Return ScanEchiquier(ReglesRassemblees.c1Pieces)

    End Function
#End Region

    Public Function WhiteToPlay() As Boolean
        Return regles.Trait = CouleurDeLaPiece.Blanc
    End Function
    'getter et setter du jeu de pièces
    Property JeuDePieces As String
        Get
            Return _JeuDePieces
        End Get
        Set(ByVal value As String)
            If value.Length = 5 Then
                _JeuDePieces = value
            End If

        End Set
    End Property
End Class
