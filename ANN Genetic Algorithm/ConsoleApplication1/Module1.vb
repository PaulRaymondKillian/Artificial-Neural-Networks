Imports System.Math

Module Module1

    Private geneticAlgorithm As New ga.Genetic_Algorithm_Controller

    Sub Main()

        For i As Integer = 0 To 99
            geneticAlgorithm.setResult(i, RandomNumber())
        Next

        geneticAlgorithm.nextGeneration()

        Dim z As String
        Console.WriteLine("Press Enter to Exit...")
        z = Console.Read()

    End Sub

    Public Function RandomNumber() As Integer
        Randomize(System.DateTime.Now.Millisecond)
        RandomNumber = Int(Rnd() * 100)
    End Function

End Module
