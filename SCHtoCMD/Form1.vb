Imports Substrate
Imports Substrate.TileEntities
Public Class Form1

    Dim inlist As New List(Of block)
    Dim inlistrev As New List(Of block)

    Dim threads As Integer = 20

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        OpenFileDialog1.Title = "Please Select a File"
        'OpenFileDialog1.InitialDirectory = 
        writefile("")
        'OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        Dim x As String = OpenFileDialog1.FileName
        'OpenFileDialog1.Dispose()
        readfile(x)
    End Sub

    Private Sub readfile(ByVal datfile As String)
        ' Open our world
        'Dim world As ImportExport.Schematic = ImportExport.Schematic.Import("C:\Users\szimmerman\Desktop\out\shack.schematic")
        Dim world As ImportExport.Schematic = ImportExport.Schematic.Import(datfile)
        Dim lastlist As New List(Of block)
        Dim xdim As Integer = world.Blocks.XDim
        Dim ydim As Integer = world.Blocks.YDim
        Dim zdim As Integer = world.Blocks.ZDim
        ' x, z, y is the most efficient order to scan blocks (not that
        ' you should care about internal detail)
        For x As Integer = 0 To xdim - 1
            For z As Integer = 0 To zdim - 1
                For y As Integer = 0 To ydim - 1
                    ' Replace the block with after if it matches before
                    If Not world.Blocks.GetID(x, y, z) = 0 Then
                        'add to list
                        Dim b As New block
                        b.block = world.Blocks.GetBlock(x, y, z)
                        b.x = x
                        b.y = y
                        b.z = z
                        If issolid(b.block) Then
                            inlist.Add(b)
                        Else
                            lastlist.Add(b)
                        End If
                    End If
                Next
            Next
        Next

        For Each x As block In lastlist
            inlist.Add(x)
        Next

        For Each x As block In inlist
            inlistrev.Add(x)
        Next
        inlistrev.Reverse()

        Label5.Text = "size: " + Convert.ToString(threads * 2) + " wide " + Convert.ToString((inlist.Count / threads) * 2 + 10) + " long"

    End Sub

    Function issolid(ByVal b As AlphaBlock) As Boolean
        Dim solids = New Integer() {1, 2, 3, 4, 5, 7, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 24, 24, 26, 30, 33, 34, 35, 36, 41, 42, 43, 44, 45, 46, 47, 48, 49, 52, 53, 54, 56, 57, 58, 60, 61, 62, 67, 73, 74, 79, 82, 84, 87, 88, 89, 95, 97, 98, 101, 102, 107, 108, 109, 110, 112, 113, 114, 116, 118, 120, 121, 123, 124, 125, 126, 128, 129, 130, 133, 134, 135, 136, 137, 138, 139, 146, 152, 153, 155, 156, 158, 159, 160, 161, 162, 163, 164, 172, 173, 174}
        For Each x As Integer In solids
            If b.ID = x Then Return True
        Next
        Return False
    End Function


    Private Sub writefile(ByVal datfile As String)
        ' Open our world
        Dim world As New ImportExport.Schematic((inlist.Count / threads) * 2 + 10, 4, threads * 2)

        Dim xoffset As Integer = xt.Text
        Dim yoffset As Integer = yt.Text
        Dim zoffset As Integer = zt.Text

        'Dim world As New ImportExport.Schematic(21, 4, 21)
        'generate floor
        For x As Integer = 0 To world.Blocks.XDim - 1
            For z As Integer = 0 To world.Blocks.ZDim - 1
                world.Blocks.SetBlock(x, 0, z, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            Next
        Next

        'generate start point
        For x As Integer = 0 To threads - 1
            'on cross section
            'ss
            'crr d
            'ssssssss
            'off cross section
            '  r 
            'crsr
            'ssdsr
            'ssssssss

            'on stripe
            'sandstone block
            world.Blocks.SetBlock(6, 2, x * 2, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            'Redstone stone trail
            world.Blocks.SetBlock(5, 1, x * 2, New AlphaBlock(Substrate.BlockType.REDSTONE_WIRE))
            world.Blocks.SetBlock(6, 1, x * 2, New AlphaBlock(Substrate.BlockType.REDSTONE_WIRE))
            'Repeater
            world.Blocks.SetBlock(3, 1, x * 2, New AlphaBlock(Substrate.BlockType.REDSTONE_REPEATER_ON, (RepeaterDelay.DELAY_1 Or RepeaterOrientation.WEST)))

            'off stripe
            world.Blocks.SetBlock(6, 1, x * 2 + 1, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            world.Blocks.SetBlock(5, 2, x * 2 + 1, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            world.Blocks.SetBlock(4, 1, x * 2 + 1, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            'Redstone stone trail
            world.Blocks.SetBlock(6, 2, x * 2 + 1, New AlphaBlock(Substrate.BlockType.REDSTONE_WIRE))
            world.Blocks.SetBlock(5, 3, x * 2 + 1, New AlphaBlock(Substrate.BlockType.REDSTONE_WIRE))
            world.Blocks.SetBlock(4, 2, x * 2 + 1, New AlphaBlock(Substrate.BlockType.REDSTONE_WIRE))
            world.Blocks.SetBlock(3, 1, x * 2 + 1, New AlphaBlock(Substrate.BlockType.REDSTONE_WIRE))
            'Repeater
            world.Blocks.SetBlock(5, 1, x * 2 + 1, New AlphaBlock(Substrate.BlockType.REDSTONE_REPEATER_ON, (RepeaterDelay.DELAY_1 Or RepeaterOrientation.WEST)))
        Next

        Dim counter As Integer = 1
        For Each blk As block In inlist

            Dim row As Integer = Math.Ceiling(counter / (threads))

            'on stripe
            'sandstone block
            Dim x = 7 + ((row - 1) * 2)
            Dim z = ((counter - 1) - ((row - 1) * threads)) * 2

            world.Blocks.SetBlock(x, 2, z, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            world.Blocks.SetBlock(x + 1, 2, z, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            'Repeater
            world.Blocks.SetBlock(x + 1, 1, z, New AlphaBlock(Substrate.BlockType.REDSTONE_REPEATER_ON, (RepeaterDelay.DELAY_1 Or RepeaterOrientation.SOUTH)))

            'off stripe
            world.Blocks.SetBlock(x, 1, z + 1, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            world.Blocks.SetBlock(x + 1, 1, z + 1, New AlphaBlock(Substrate.BlockType.SANDSTONE))
            'Repeater
            world.Blocks.SetBlock(x + 1, 2, z + 1, New AlphaBlock(Substrate.BlockType.REDSTONE_REPEATER_ON, (RepeaterDelay.DELAY_1 Or RepeaterOrientation.SOUTH)))

            'on
            Dim ab1 As New Substrate.AlphaBlock(BlockType.COMMAND_BLOCK)
            Dim xx1 As TileEntityControl = ab1.GetTileEntity
            xx1.Command = "setblock " + Convert.ToString(blk.x + xoffset) + " " + Convert.ToString(blk.y + yoffset) + " " + Convert.ToString(blk.z + zoffset) + " " + Convert.ToString(blk.block.ID) + " " + Convert.ToString(blk.block.Data)
            ab1.SetTileEntity(xx1)
            world.Blocks.SetBlock(x, 1, z, ab1)
            'off
            Dim ab2 As New Substrate.AlphaBlock(BlockType.COMMAND_BLOCK)
            Dim xx2 As TileEntityControl = ab2.GetTileEntity
            xx2.Command = "setblock " + Convert.ToString(inlistrev(counter - 1).x + xoffset) + " " + Convert.ToString(inlistrev(counter - 1).y + yoffset) + " " + Convert.ToString(inlistrev(counter - 1).z + zoffset) + " 00"
            ab2.SetTileEntity(xx2)
            world.Blocks.SetBlock(x, 2, z + 1, ab2)

            counter += 1
        Next

        Dim saveFileDialog1 As New SaveFileDialog()

        saveFileDialog1.Filter = "schematic files (*.schematic)|*.schematic|All files (*.*)|*.*"
        saveFileDialog1.FilterIndex = 2
        saveFileDialog1.RestoreDirectory = True

        If saveFileDialog1.ShowDialog() = DialogResult.OK Then
            If Not saveFileDialog1.FileName.Contains(".schematic") Then
                world.Export(saveFileDialog1.FileName + ".schematic")
            Else
                world.Export(saveFileDialog1.FileName)
            End If
        End If

        Me.Close()
    End Sub

    Class block
        Public block As AlphaBlock
        Public x As Integer
        Public y As Integer
        Public z As Integer
    End Class


    Private Sub TextBox4_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tt.TextChanged
        threads = tt.Text
        Label5.Text = "size: " + Convert.ToString(threads * 2) + " wide " + Convert.ToString((inlist.Count / threads) * 2 + 10) + " long"
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        OpenFileDialog1.Title = "Please Select a File"
        OpenFileDialog1.ShowDialog()
    End Sub
End Class
