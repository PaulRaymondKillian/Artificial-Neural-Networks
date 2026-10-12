Option Strict On

Imports System.IO
Imports System.Text

Public Class Network

    Private _firstInputNeuron As Neuron
    Private _firstHiddenNeuron As Neuron
    Private _firstOutputNeuron As Neuron
    Private _currentCalcNeuron As Neuron
    Private _previousCalcNeuron As Neuron
    Private _lastNeuron As Neuron

    Private _numInputNeurons As Integer
    Private _numHiddenNeurons As Integer
    Private _numHiddenLayers As Integer
    Private _numOutputNeurons As Integer

    Private _doneCalculating As Boolean = True

    Private Const e As Double = 2.71828183

    Public ReadOnly Property firstInputNeuron() As Neuron
        Get
            Return _firstInputNeuron
        End Get
    End Property

    Public ReadOnly Property firstHiddenNeuron() As Neuron
        Get
            Return _firstHiddenNeuron
        End Get
    End Property

    Public ReadOnly Property firstOutputNeuron() As Neuron
        Get
            Return _firstOutputNeuron
        End Get
    End Property

    Public ReadOnly Property numInputNeurons() As Integer
        Get
            Return _numInputNeurons
        End Get
    End Property

    Public ReadOnly Property numHiddenNeurons() As Integer
        Get
            Return _numHiddenNeurons
        End Get
    End Property

    Public ReadOnly Property numHiddenLayers() As Integer
        Get
            Return _numHiddenLayers
        End Get
    End Property

    Public ReadOnly Property numOutputNeurons() As Integer
        Get
            Return _numOutputNeurons
        End Get
    End Property

    Public ReadOnly Property numTotalNeurons() As Integer
        Get
            Return _numInputNeurons + 1 + ((_numHiddenNeurons + 1) * _numHiddenLayers) + _numOutputNeurons
        End Get
    End Property

    Public ReadOnly Property numTotalLayers() As Integer
        Get
            Return 2 + _numHiddenLayers
        End Get
    End Property

    'This property is used, primarily by the viewer, to determine the 'width' of the network to find how large to make the neuron circles
    Public ReadOnly Property numNetworkNeuronWidth() As Integer
        Get
            Dim x As Integer

            If _numInputNeurons >= _numHiddenNeurons And _numInputNeurons >= _numOutputNeurons Then
                x = _numInputNeurons + 1
            ElseIf _numHiddenNeurons >= _numInputNeurons And _numHiddenNeurons >= _numOutputNeurons Then
                x = _numHiddenNeurons + 1
            Else
                x = _numOutputNeurons
            End If

            Return x
        End Get
    End Property

    Public ReadOnly Property numWeights() As Integer
        Get
            Return (numHiddenNeurons * (numInputNeurons + 1)) + ((numHiddenLayers - 1) * (numHiddenNeurons * (numHiddenNeurons + 1))) + (numOutputNeurons * (numHiddenNeurons + 1))
        End Get
    End Property

    Public ReadOnly Property currentCalcNeuron() As Neuron
        Get
            Return _currentCalcNeuron
        End Get
    End Property

    Public ReadOnly Property previousCalcNeuron() As Neuron
        Get
            Return _previousCalcNeuron
        End Get
    End Property


    Public Sub New(ByVal numInputNeurons As Integer, ByVal numHiddenNeurons As Integer, ByVal numHiddenLayers As Integer, ByVal numOutputNeurons As Integer)
        If numInputNeurons < 1 Or numHiddenNeurons < 1 Or numHiddenLayers < 1 Or numOutputNeurons < 1 Then
            'MsgBox("Error - Class: Network, Method: New(), Desc: Invalid parameters inputted, must be >1")
            Throw New Exception("Error - Class: Network, Method: New(), Desc: Invalid parameters inputted, must be >1")
            Exit Sub
        End If

        BuildNetwork(numInputNeurons, numHiddenNeurons, numHiddenLayers, numOutputNeurons)

    End Sub

    Public Sub New(ByVal numInputNeurons As Integer, ByVal numHiddenNeurons As Integer, ByVal numOutputNeurons As Integer)
        If numInputNeurons < 1 Or numHiddenNeurons < 1 Or numOutputNeurons < 1 Then
            'MsgBox("Error - Class: Network, Method: New(), Desc: Invalid parameters inputted, must be >1")
            Throw New Exception("Error - Class: Network, Method: New(), Desc: Invalid parameters inputted, must be >1")
            Exit Sub
        End If

        BuildNetwork(numInputNeurons, numHiddenNeurons, 1, numOutputNeurons)

    End Sub

    Public Sub New(ByVal numInputNeurons As Integer, ByVal numOutputNeurons As Integer)
        If numInputNeurons < 1 Or numOutputNeurons < 1 Then
            'MsgBox("Error - Class: Network, Method: New(), Desc: Invalid parameters inputted, must be >1")
            Throw New Exception("Error - Class: Network, Method: New(), Desc: Invalid parameters inputted, must be >1")
            Exit Sub
        End If

        BuildNetwork(numInputNeurons, 6, 1, numOutputNeurons)

    End Sub

    Public Sub BuildNetwork(ByVal numInputNeurons As Integer, ByVal numHiddenNeurons As Integer, ByVal numHiddenLayers As Integer, ByVal numOutputNeurons As Integer)
        _numInputNeurons = numInputNeurons
        _numHiddenNeurons = numHiddenNeurons
        _numHiddenLayers = numHiddenLayers
        _numOutputNeurons = numOutputNeurons


        Dim currentNeuron As Neuron
        Dim previousNeuron As Neuron
        Dim startOfPreviousLayer As Neuron
        Dim currentNeuronIndex As Integer = 1


        'Build Input Nodes
        For i As Integer = 1 To _numInputNeurons

            currentNeuron = New Neuron
            currentNeuron.type = Neuron.NeuronType.InputNeuron
            currentNeuron.ID = "I-" & currentNeuronIndex

            If i = 1 Then
                _firstInputNeuron = currentNeuron   'Set first neuron
            Else
                previousNeuron.nextNeuronLink = currentNeuron
            End If

            previousNeuron = currentNeuron
            currentNeuronIndex = currentNeuronIndex + 1
        Next

        'Build input layer bias
        currentNeuron = New Neuron
        currentNeuron.type = Neuron.NeuronType.BiasNeuron
        currentNeuron.ID = "IB-" & currentNeuronIndex
        previousNeuron.nextNeuronLink = currentNeuron
        previousNeuron = currentNeuron
        currentNeuronIndex = currentNeuronIndex + 1

        For i As Integer = 1 To _numHiddenLayers '+ 1 needed?
            For j As Integer = 1 To numHiddenNeurons '+ 1 needed?
                currentNeuron = New Neuron
                currentNeuron.type = Neuron.NeuronType.HiddenNeuron
                currentNeuron.ID = "H" & i & "-" & currentNeuronIndex
                previousNeuron.nextNeuronLink = currentNeuron

                If i = 1 Then

                    If j = 1 Then
                        _firstHiddenNeuron = currentNeuron
                        _currentCalcNeuron = _firstHiddenNeuron
                    End If
                    'If Neuron Links are the Input Layer
                    Dim tempCurrentNeuron As Neuron
                    tempCurrentNeuron = _firstInputNeuron
                    For k = 1 To numInputNeurons + 1 'The +1 is for the Bias
                        currentNeuron.AddWeight(tempCurrentNeuron)
                        tempCurrentNeuron = tempCurrentNeuron.nextNeuronLink
                    Next
                Else
                    'If Neuron Links are the below Hidden Layer
                    Dim tempCurrentNeuron As Neuron
                    tempCurrentNeuron = _firstHiddenNeuron

                    'Scan through hidden layers to get to the proper one (if second hidden layer, should bypass this)
                    For k = 1 To ((i - 2) * (numHiddenNeurons + 1))
                        tempCurrentNeuron = tempCurrentNeuron.nextNeuronLink
                    Next

                    For k = 1 To numHiddenNeurons + 1
                        currentNeuron.AddWeight(tempCurrentNeuron)
                        tempCurrentNeuron = tempCurrentNeuron.nextNeuronLink
                    Next

                    'You will need to implement this layer by using modulus on the number of hidden neurons per each hidden
                    'layer to find the starting point of the first node of the previous hidden layer, and then add weights 
                    'the number of neurons per each hidden layer
                    'MsgBox("Not Implemented Yet - Class: Network, Method: BuildNetwork, Descr: This Functionality Is Not Implemented Yet! Multi-Layer networks are not yet supported")
                End If

                previousNeuron = currentNeuron
                currentNeuronIndex = currentNeuronIndex + 1

            Next

            'Build hidden layer bias
            currentNeuron = New Neuron
            currentNeuron.type = Neuron.NeuronType.BiasNeuron
            currentNeuron.ID = "H" & i & "B-" & currentNeuronIndex
            previousNeuron.nextNeuronLink = currentNeuron
            previousNeuron = currentNeuron
            currentNeuronIndex = currentNeuronIndex + 1

        Next

        'Build Output Layer

        'To set the node links for the output layer, we need a reference to 
        'the first node of the below hidden layer
        Dim tempLastHiddenLayerFirstNeuron As Neuron
        'We find the first node of the prevous layer be going back the number of nodes in the previous layer plus 1 (+1 for the Bias)
        tempLastHiddenLayerFirstNeuron = getNeuronFromIndex(currentNeuronIndex - _numHiddenNeurons - 1)

        For i As Integer = 1 To _numOutputNeurons

            currentNeuron = New Neuron
            currentNeuron.type = Neuron.NeuronType.OutputNeuron
            currentNeuron.ID = "O-" & currentNeuronIndex

            If i = 1 Then
                _firstOutputNeuron = currentNeuron
            End If

            Dim tempCurrentNeuron As Neuron
            tempCurrentNeuron = tempLastHiddenLayerFirstNeuron
            For k = 1 To numHiddenNeurons + 1 'The +1 is for the Bias
                currentNeuron.AddWeight(tempCurrentNeuron)
                tempCurrentNeuron = tempCurrentNeuron.nextNeuronLink
            Next

            previousNeuron.nextNeuronLink = currentNeuron
            previousNeuron = currentNeuron
            currentNeuronIndex = currentNeuronIndex + 1

        Next

        _lastNeuron = currentNeuron

    End Sub

    Public Function Activation(ByVal sum As Double) As Double

    End Function

    Public Function getNeuronFromIndex(ByVal index As Integer) As Neuron
        'Output message box error indicating invalid input was used. In the future replace this with better error reporting and/or exception handling
        If index < 1 Then
            'MsgBox("Error - Class: Network, Method: getNeuronFromIndex, Desc: Index of neuron to retrieve must be greater than 0, value inputted = " & index)
            Throw New Exception("Error - Class: Network, Method: getNeuronFromIndex, Desc: Index of neuron to retrieve must be greater than 0, value inputted = " & index)
            Exit Function
        End If

        Dim tempNeuron As Neuron
        tempNeuron = _firstInputNeuron

        For i As Integer = 1 To index - 1
            tempNeuron = tempNeuron.nextNeuronLink
        Next

        Return tempNeuron
    End Function

    Public Function getNeuron(ByVal layer As Integer, ByVal index As Integer) As Neuron
        Dim currentNeuron As Neuron

        If layer = 0 Then
            currentNeuron = _firstInputNeuron
            For i As Integer = 0 To index - 1
                currentNeuron = currentNeuron.nextNeuronLink
            Next
        ElseIf layer > 0 And layer < (1 + numHiddenLayers) Then
            currentNeuron = _firstHiddenNeuron
            For i As Integer = 0 To ((layer - 2) * (numHiddenNeurons + 1)) - 1
                currentNeuron = currentNeuron.nextNeuronLink
            Next
            For i As Integer = 0 To index - 1
                currentNeuron = currentNeuron.nextNeuronLink
            Next
        Else
            currentNeuron = _firstOutputNeuron
            For i As Integer = 0 To index - 1
                currentNeuron = currentNeuron.nextNeuronLink
            Next
        End If

        Return currentNeuron

    End Function

    Public Sub InputWeights(ByVal weights() As Double)
        If weights.Length <> numWeights Then
            'MsgBox("Error - Class: Network, Method: InputWeights( Double() ), Descr: Invalid number of weights inputted: " & weights.Length & " inputted; " & numWeights & " needed.")
            Throw New Exception("Error - Class: Network, Method: InputWeights( Double() ), Descr: Invalid number of weights inputted: " & weights.Length & " inputted; " & numWeights & " needed.")
            Exit Sub
        End If

        Dim currentNeuron As Neuron
        Dim index As Integer = 0
        currentNeuron = firstHiddenNeuron

        While Not currentNeuron Is Nothing
            If currentNeuron.type <> Neuron.NeuronType.BiasNeuron Then
                For i As Integer = 0 To (currentNeuron.weightsVector.totalWeights - 1)
                    currentNeuron.weightsVector.weights(i).value = weights(index)
                    index = index + 1
                Next
            End If
            currentNeuron = currentNeuron.nextNeuronLink
        End While

    End Sub

    Public Sub InputWeights(ByVal sWeights As String)

        Dim weights(numWeights - 1) As Double
        Dim tempWeight() As String

        tempWeight = sWeights.Split(","c)

        If tempWeight.Length <> numWeights Then
            'MsgBox("Error - Class: Network, Method: InputWeights( String ), Descr: Invalid number of weights inputted: " & tempWeight.Length & " inputted; " & numWeights & " needed.")
            Throw New Exception("Error - Class: Network, Method: InputWeights( String ), Descr: Invalid number of weights inputted: " & tempWeight.Length & " inputted; " & numWeights & " needed.")
            Exit Sub
        End If

        For i As Integer = 0 To numWeights - 1
            If IsNumeric(tempWeight(i)) = True Then
                weights(i) = Convert.ToDouble(tempWeight(i))
            Else
                'MsgBox("Error - Class: Network, Method: InputWeights( String ), Descr: Weight inputted was not numeric: weight=" & tempWeight(i).ToString)
                Throw New Exception("Error - Class: Network, Method: InputWeights( String ), Descr: Weight inputted was not numeric: weight=" & tempWeight(i).ToString)
                Exit Sub
            End If

        Next

        InputWeights(weights)

    End Sub

    Public Sub InputWeightsFromFile(ByVal filePath As String)

        Dim sr As StreamReader = File.OpenText(filePath)

        Dim sbWeights As New StringBuilder("")

        While Not sr.EndOfStream
            sbWeights.Append(sr.ReadLine)

            If Not sr.EndOfStream Then
                sbWeights.Append(",")
            End If

        End While

        sr.Close()

        InputWeights(sbWeights.ToString)


    End Sub

    Public Sub ExportWeightsToFile(ByVal fileName As String)
        Dim sw As StreamWriter = File.CreateText(fileName)

        Dim currentNeuron As Neuron
        currentNeuron = _firstHiddenNeuron

        Do
            If currentNeuron.type <> Neuron.NeuronType.BiasNeuron Then
                For i As Integer = 0 To currentNeuron.weightsVector.totalWeights - 1
                    sw.WriteLine(currentNeuron.weightsVector.weights(i).value)
                Next
            End If
            currentNeuron = currentNeuron.nextNeuronLink
        Loop Until currentNeuron Is Nothing

        sw.Close()

    End Sub

    Public Function LoadNetworkFromFile(ByVal filename As String) As Network
        Dim sr As StreamReader = File.OpenText(filename)
        Dim numInputNeurons, numHiddenNeurons, numHiddenLayers, numOutputNeurons As Integer
        Dim ANN As Network

        numInputNeurons = Convert.ToInt32(sr.ReadLine)
        numHiddenNeurons = Convert.ToInt32(sr.ReadLine)
        numHiddenLayers = Convert.ToInt32(sr.ReadLine)
        numOutputNeurons = Convert.ToInt32(sr.ReadLine)

        ANN = New Network(numInputNeurons, numHiddenNeurons, numHiddenLayers, numOutputNeurons)

        Dim currentNeuron As Neuron
        currentNeuron = ANN.firstHiddenNeuron

        Do
            If currentNeuron.type <> Neuron.NeuronType.BiasNeuron Then
                For i As Integer = 0 To currentNeuron.weightsVector.totalWeights - 1
                    currentNeuron.weightsVector.weights(i).value = Convert.ToDouble(sr.ReadLine)
                Next
            End If
            currentNeuron = currentNeuron.nextNeuronLink
        Loop Until currentNeuron Is Nothing

        Return ANN

    End Function

    Public Sub SaveNetworkToFile(ByVal filename As String)

        Dim sw As StreamWriter = File.CreateText(filename)

        sw.WriteLine(_numInputNeurons)
        sw.WriteLine(_numHiddenNeurons)
        sw.WriteLine(_numHiddenLayers)
        sw.WriteLine(_numOutputNeurons)

        Dim currentNeuron As Neuron
        currentNeuron = _firstHiddenNeuron

        Do
            If currentNeuron.type <> Neuron.NeuronType.BiasNeuron Then
                For i As Integer = 0 To currentNeuron.weightsVector.totalWeights - 1
                    sw.WriteLine(currentNeuron.weightsVector.weights(i).value)
                Next
            End If
            currentNeuron = currentNeuron.nextNeuronLink
        Loop Until currentNeuron Is Nothing

        sw.Close()

    End Sub

    Public Sub calculateNextNeuron()
        _doneCalculating = False
        If Not _currentCalcNeuron Is Nothing Then
            _currentCalcNeuron.calculateNeuron()
            _previousCalcNeuron = _currentCalcNeuron
            _currentCalcNeuron = _currentCalcNeuron.nextNeuronLink
            If Not _currentCalcNeuron Is Nothing AndAlso _currentCalcNeuron.type = Neuron.NeuronType.BiasNeuron Then
                '_previousCalcNeuron = _currentCalcNeuron
                _currentCalcNeuron = _currentCalcNeuron.nextNeuronLink
            End If
        End If

        If _currentCalcNeuron Is Nothing Then
            'Reset _currentCalcNeuron to beginning node to be calculated
            _currentCalcNeuron = _firstHiddenNeuron
            _previousCalcNeuron = Nothing
            _doneCalculating = True
        End If
    End Sub

    Public Sub calculateNextLayer()
        _doneCalculating = False
        'If Not _currentCalcNeuron Is Nothing Then
        Do
            calculateNextNeuron()
        Loop While (_doneCalculating = False) AndAlso (_previousCalcNeuron.nextNeuronLink.type <> Neuron.NeuronType.BiasNeuron)
        'End If
    End Sub

    Public Sub calculateAll()
        _doneCalculating = False
        While _doneCalculating = False
            calculateNextLayer()
        End While

        'Reset _currentCalcNeuron to beginning node to be calculated
        '_currentCalcNeuron = _firstHiddenNeuron

    End Sub

    Public Sub setInputValue(ByVal index As Integer, ByVal value As Double)
        If index <= numInputNeurons And index > 0 Then
            getNeuronFromIndex(index).value = value
        Else
            'MsgBox("Error - Class: Network, Method: setInputValue, Descr: Input neuron index inputted not valid: index inputed=" & index & ", max index=" & numInputNeurons)
            Throw New Exception("Error - Class: Network, Method: setInputValue, Descr: Input neuron index inputted not valid: index inputed=" & index & ", max index=" & numInputNeurons)
        End If
    End Sub

    Public Sub resetNetwork()
        Dim currentNeuron As Neuron
        currentNeuron = _firstHiddenNeuron

        Do While Not currentNeuron Is Nothing
            currentNeuron.resetValue()
            currentNeuron = currentNeuron.nextNeuronLink
        Loop

        _currentCalcNeuron = _firstHiddenNeuron
        _previousCalcNeuron = Nothing


    End Sub

    Public Function getWeightTotal() As Double

        Dim weightTotal As Double = 0
        Dim currentNeuron As Neuron
        currentNeuron = _firstHiddenNeuron

        Do
            If currentNeuron.type <> Neuron.NeuronType.BiasNeuron Then
                For i As Integer = 0 To currentNeuron.weightsVector.totalWeights - 1
                    weightTotal += currentNeuron.weightsVector.weights(i).value
                Next
            End If
            currentNeuron = currentNeuron.nextNeuronLink
        Loop Until currentNeuron Is Nothing

        Return weightTotal
    End Function

    Public Function getMaxWeightValue() As Double

        Dim maxWeight As Double = Double.MinValue
        Dim currentNeuron As Neuron
        currentNeuron = _firstHiddenNeuron

        Do
            If currentNeuron.type <> Neuron.NeuronType.BiasNeuron Then
                For i As Integer = 0 To currentNeuron.weightsVector.totalWeights - 1
                    If currentNeuron.weightsVector.weights(i).value >= maxWeight Then
                        maxWeight = currentNeuron.weightsVector.weights(i).value
                    End If
                Next
            End If
            currentNeuron = currentNeuron.nextNeuronLink
        Loop Until currentNeuron Is Nothing

        Return maxWeight

    End Function

    Public Function getMinWeightValue() As Double

        Dim minWeight As Double = Double.MaxValue
        Dim currentNeuron As Neuron
        currentNeuron = _firstHiddenNeuron

        Do
            If currentNeuron.type <> Neuron.NeuronType.BiasNeuron Then
                For i As Integer = 0 To currentNeuron.weightsVector.totalWeights - 1
                    If currentNeuron.weightsVector.weights(i).value <= minWeight Then
                        minWeight = currentNeuron.weightsVector.weights(i).value
                    End If
                Next
            End If
            currentNeuron = currentNeuron.nextNeuronLink
        Loop Until currentNeuron Is Nothing

        Return minWeight

    End Function

    Public Function getNumZeroWeights(ByVal range As Double) As Integer

        Dim numZeroes As Integer = 0
        Dim currentNeuron As Neuron
        currentNeuron = _firstHiddenNeuron

        Do
            If currentNeuron.type <> Neuron.NeuronType.BiasNeuron Then
                For i As Integer = 0 To currentNeuron.weightsVector.totalWeights - 1
                    If currentNeuron.weightsVector.weights(i).value >= -range And currentNeuron.weightsVector.weights(i).value <= range Then
                        numZeroes += 1
                    End If
                Next
            End If
            currentNeuron = currentNeuron.nextNeuronLink
        Loop Until currentNeuron Is Nothing

        Return numZeroes

    End Function

End Class
