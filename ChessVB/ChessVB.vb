Imports System.Drawing.Imaging

Public Class ChessVB

#Region "Déclaration des variables pour toutes les images"
    Dim pionBlanc As New Bitmap(My.Resources.PionBlanc)
    Dim tourBlanche As New Bitmap(My.Resources.TourBlanche)
    Dim cavalierBlanc As New Bitmap(My.Resources.CavalierBlanc)
    Dim fouBlanc As New Bitmap(My.Resources.FouBlanc)
    Dim dameBlanche As New Bitmap(My.Resources.DameBlanche)
    Dim roiBlanc As New Bitmap(My.Resources.RoiBlanc)

    Dim pionNoir As New Bitmap(My.Resources.PionNoir)
    Dim tourNoire As New Bitmap(My.Resources.TourNoire)
    Dim cavalierNoir As New Bitmap(My.Resources.CavalierNoir)
    Dim fouNoir As New Bitmap(My.Resources.FouNoir)
    Dim dameNoire As New Bitmap(My.Resources.DameNoire)
    Dim roiNoir As New Bitmap(My.Resources.RoiNoir)

    Dim echiquier As New Bitmap(My.Resources.Echiquier)
    Dim hautBas As New Bitmap(My.Resources.HautBas)
    Dim cotes As New Bitmap(My.Resources.Cotes)

    Dim rondVert As New Bitmap(My.Resources.Vert)
    Dim rondRouge As New Bitmap(My.Resources.Rouge)
    Dim rondMauve As New Bitmap(My.Resources.Mauve)
    Dim attention As New Bitmap(My.Resources.Attention)
    Dim croix As New Bitmap(My.Resources.Croix)
#End Region
#Region "Déclaration des variables"
    Private Echiquier10x10(100) As Char 'pour correspondre avec Mouvements
    Dim TaillePiece As Integer  'Va servir à adapter la taille de la pièce à une case donnée
    Dim echiquierFond As New Bitmap(My.Resources.Echiquier)  'Pour redimensionner l'échiquier
    Dim g As Graphics = Graphics.FromImage(echiquierFond)    'instancie echiquierFond en tant qu'objet Graphics pour pouvoir modifier ses paramètres
    Public Position As Mouvements 'Retient la position pour pouvoir la (re)dessiner
#End Region
#Region "Toutes les fonctions et méthodes de dessin"
    Private Sub DessineEchiquier()
        Dim rect As Rectangle
        Dim pt As Point

        PictureBox1.Height = Me.ClientSize.Height - 20
        PictureBox1.Width = Me.ClientSize.Height - 20

        PictureBox1.Image = New Bitmap(PictureBox1.Width, PictureBox1.Height)

        echiquierFond = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        g = Graphics.FromImage(echiquierFond)

        pt.X = 1 : pt.Y = 1 : g.DrawImage(hautBas, pt)                        'dessine le bord haut
        pt.X = 1 : pt.Y = PictureBox1.Height - 17 : g.DrawImage(hautBas, pt)  'dessine le bord bas
        pt.X = 1 : pt.Y = 1 : g.DrawImage(cotes, pt)                        'dessine le bord gauche
        pt.X = PictureBox1.Width - 17 : pt.Y = 1 : g.DrawImage(cotes, pt)   'dessine le bord droit

        rect.X = 0 : rect.Y = 0
        rect.Width = PictureBox1.Width - 1 : rect.Height = PictureBox1.Height - 1
        g.DrawRectangle(Pens.Black, rect)                                   'dessine la ligne noire autour des bords(extérieur) 

        rect.X = 17 : rect.Y = 17
        rect.Width = PictureBox1.Width - 35 : rect.Height = PictureBox1.Height - 35
        g.DrawRectangle(Pens.Black, rect)                                   'dessine la ligne noire autour des bords(intérieur)

        rect.X = 18 : rect.Y = 18
        rect.Width = PictureBox1.Width - 36 : rect.Height = PictureBox1.Height - 36
        g.DrawImage(echiquier, rect)                                           'dessine l'échiquier redimensionné

        TaillePiece = (PictureBox1.Height - 36) / 8     'récupère la taille d'une case

        'lvMoves.Top = 10
        'lvMoves.Left = PictureBox1.Left + PictureBox1.Width + 10
        'lvMoves.Height = PictureBox1.Height

        PictureBox1.Image = echiquierFond

        ScanEchiquier(Position.GetPosition)  'récupère la position des pièces(l'état complet de l'échiquier)

        Dim Piece As Char
        For i = 11 To 88                            'chaque nombre entre 11 et 88 correspond à une case : comme si c'était un tableau 8 sur 8
            Piece = Echiquier10x10(i)             'exemple 11 = coordonnées (1,1), 78 = (7,8), 91 = hors de l'échiquier
            If Piece <> " " And Piece <> "*" Then 'récupère la position d'une pièce pour la placer sur l'échiquier
                DessinePiece(i, Piece)        'place la pièce sur l'échiquier
            End If
        Next

    End Sub
    Private Function bmpPiece(ByVal name As Char) As Bitmap
        Select Case name
            Case "P"
                Return pionBlanc
            Case "R"
                Return tourBlanche
            Case "N"
                Return cavalierBlanc
            Case "B"
                Return fouBlanc
            Case "Q"
                Return dameBlanche
            Case "K"
                Return roiBlanc

            Case "p"
                Return pionNoir
            Case "r"
                Return tourNoire
            Case "n"
                Return cavalierNoir
            Case "b"
                Return fouNoir
            Case "q"
                Return dameNoire
            Case "k"
                Return roiNoir

            Case "1"
                Return rondRouge
            Case "2"
                Return rondVert
            Case "3"
                Return rondMauve
            Case "4"
                Return croix


        End Select
    End Function
    Private Sub DessinePiece(ByVal sqIndex As Byte, ByVal Initiale As Char)
        Dim rect As Rectangle
        rect.X = Xsqi(sqIndex)
        rect.Y = Ysqi(sqIndex)
        rect.Width = TaillePiece
        rect.Height = TaillePiece

        g.DrawImage(bmpPiece(Initiale), rect)

    End Sub
    Private Function Xsqi(ByVal sqi As Byte) As Integer
        Dim colonne As Byte
        Dim ligne As Byte

        colonne = sqi Mod 10
        ligne = sqi \ 10

        Return 18 + (colonne - 1) * TaillePiece

    End Function
    Private Function Ysqi(ByVal sqi As Byte) As Integer
        Dim colonne As Byte
        Dim ligne As Byte

        colonne = sqi Mod 10
        ligne = sqi \ 10

        Return 18 + (8 - ligne) * TaillePiece

    End Function
    Private Sub PutSymbol(ByVal sqIndex As Byte, ByVal Initiale As Char)
        Dim rect As Rectangle
        Dim p As Graphics = PictureBox1.CreateGraphics
        rect.X = Xsqi(sqIndex)
        rect.Y = Ysqi(sqIndex)
        rect.Width = TaillePiece
        rect.Height = TaillePiece

        p.DrawImage(bmpPiece(Initiale), rect)

    End Sub

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
#Region "Evènements de l'Echiquier"
    Private Sub ChessVB_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        DessineEchiquier() 'Méthode de dessin de l'échiquer
    End Sub
    Private Sub ChessVB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Top = 10        'Place le bord
        PictureBox1.Left = 10       'Place le bord
        Position = New Mouvements()  'instancie la classe Mouvements
        Position.JeuDePieces = "TCFDR" 'détermine le jeu de pièces (+plus la langue)
    End Sub
#End Region
End Class
