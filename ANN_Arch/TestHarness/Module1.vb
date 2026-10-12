

Module Module1

    Sub Main()
        Dim ANN As New ANN_Arch.Network(3, 3, 3, 2)

        'Console.WriteLine(ANN.numWeights)

        ''Dim arr(19) As Double
        ''For i As Integer = 0 To 19
        ''    arr(i) = i
        ''Next

        ''ANN.InputWeights(arr)

        ''Dim s As String = "1,10,100,123,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,20"
        ''ANN.InputWeights(s)

        'ANN.InputWeightsFromFile("C:\a.txt")
        'ANN.setInputValue(1, 1)
        'ANN.setInputValue(2, 1)
        'ANN.setInputValue(3, 1)
        'ANN.calculateAll()

        'For i As Integer = 1 To (ANN.numInputNeurons + 1 + (ANN.numHiddenLayers * (ANN.numHiddenNeurons + 1)) + ANN.numOutputNeurons)
        '    Console.WriteLine(ANN.getNeuronFromIndex(i).getNeuronInfo())
        'Next


        'Console.WriteLine(ANN.getWeightTotal())

        Console.WriteLine(ANN.getNeuron(1, 2).value)


        Console.WriteLine("Press Enter to Exit...")
        Dim z As String = Console.ReadLine
    End Sub

End Module