
Public Class Neuron
    Private _value As Double
    Private _nextNeuronLink As Neuron
    Private _type As NeuronType
    Private _ID As String
    Private _weightsVector As WeightsVector

    Public Enum NeuronType As Integer
        UninitializedNeuron = 0
        InputNeuron = 1
        HiddenNeuron = 2
        OutputNeuron = 3
        BiasNeuron = 4
    End Enum

    Public Property value() As Double
        Get
            Return _value
        End Get
        Set(ByVal value As Double)
            If _type = NeuronType.InputNeuron Then
                _value = value
            Else
                'MsgBox("Error - Class: Neuron, Method: Property value(), Desc: Attempted to set value for node that is not an input node. You can only set the value for input nodes!")
                Throw New Exception("Error - Class: Neuron, Method: Property value(), Desc: Attempted to set value for node that is not an input node. You can only set the value for input nodes!")
            End If
        End Set
    End Property

    Public Sub resetValue()
        _value = 0
    End Sub

    Public Property nextNeuronLink() As Neuron
        Get
            Return _nextNeuronLink
        End Get
        Set(ByVal value As Neuron)
            _nextNeuronLink = value
        End Set
    End Property

    Public Property type() As NeuronType
        Get
            Return _type
        End Get
        Set(ByVal value As NeuronType)
            If value = NeuronType.BiasNeuron Then
                _value = 1
            End If
            _type = value
        End Set
    End Property

    Public Property ID() As String
        Get
            Return _ID
        End Get
        Set(ByVal value As String)
            _ID = value
        End Set
    End Property

    Public ReadOnly Property weightsVector() As WeightsVector
        Get
            Return _weightsVector
        End Get
    End Property

    Public Sub New()
        _value = 0
        _type = NeuronType.UninitializedNeuron
        _weightsVector = New WeightsVector()
    End Sub

    Public Sub AddWeight(ByVal tempNeuron As Neuron)
        _weightsVector.AddWeight(tempNeuron)
    End Sub

    Public Function calculateNeuron() As Double
        'perform neuron calculations
        _value = 0  'Reset value to 0

        For i As Integer = 0 To _weightsVector.totalWeights - 1
            _value = _value + _weightsVector.weights(i).value * _weightsVector.weights(i).linkedNeuron.value
        Next

        Return _value
    End Function


    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Begin Debugging Functions
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    Public Function getNeuronInfo() As String
        Dim s As String

        Dim nextNeuronLinkID As String
        If Not nextNeuronLink Is Nothing Then
            nextNeuronLinkID = nextNeuronLink.ID
        Else
            nextNeuronLinkID = "Nothing"
        End If

        s = _ID & "   Type: " & _type & "   Value: " & _value & vbCrLf
        s = s & "NextNodeLink: " & nextNeuronLinkID & "   Weights: " & _weightsVector.totalWeights & vbCrLf

        If _weightsVector.totalWeights > 0 Then
            For i As Integer = 0 To _weightsVector.totalWeights - 1
                s = s & _weightsVector.weights(i).linkedNeuron.ID & "=" & _weightsVector.weights(i).value & " | "
            Next
        End If

        s = s & vbCrLf & "#########################" & vbCrLf

        Return s
    End Function


End Class

Public Class WeightsVector
    Private _weights() As Weight
    Private _totalWeights As Integer

    Public ReadOnly Property totalWeights() As Integer
        Get
            Return _totalWeights
        End Get
    End Property

    Public Property weights() As Weight()
        Get
            Return _weights
        End Get
        Set(ByVal value As Weight())
            _weights = value
        End Set
    End Property

    'Public Property weights(ByVal index As Integer)
    '    Get
    '        If index <= _totalWeights - 1 Then
    '            Return weights(index)
    '        Else
    '            MsgBox("Error - Class: WeightsVector, Method: Property weights( integer ), Desc: Invalid weight index inputted: max value = " & _totalWeights - 1 & ", inputted= " & index)
    '        End If
    '    End Get
    '    Set(ByVal value)
    '        weights(index) = value
    '    End Set
    'End Property

    Public Sub New()
        'ReDim _weights(numWeights)
        '_totalWeights = numWeights

        ''Initialize each weight
        'For i As Integer = 0 To _totalWeights - 1
        '    _weights(i) = New Weight()
        'Next i

        ReDim _weights(0)
        _totalWeights = 0

    End Sub

    Public Sub AddWeight(ByVal tempNeuron As Neuron)
        _totalWeights = _totalWeights + 1

        'There is a huge performance hit with 'Redim Preserve' on large arrays and therefore this should probably
        'be changed back to using a linked list of weights, or some other array logic that will have better efficency
        ReDim Preserve _weights(_totalWeights)

        _weights(_totalWeights - 1) = New Weight
        _weights(_totalWeights - 1).linkedNeuron = tempNeuron

    End Sub

    Public Function getWeightValue(ByVal index As Integer)
        Return _weights(index).value
    End Function

    Public Function getWeightsVectorOutput() As Double
        Dim weightedValue As Double

        For i As Integer = 0 To _totalWeights - 1
            weightedValue = weightedValue + _weights(i).getWeightedValue
        Next i

        Return weightedValue

    End Function

End Class

Public Class Weight
    Private _value As Double
    Private _linkedNeuron As Neuron

    Public Property value() As Double
        Get
            Return _value
        End Get
        Set(ByVal value As Double)
            _value = value
        End Set
    End Property

    Public Property linkedNeuron() As Neuron
        Get
            Return _linkedNeuron
        End Get
        Set(ByVal value As Neuron)
            _linkedNeuron = value
        End Set
    End Property

    Public Sub New()
        _value = 99
    End Sub

    Public Function getWeightedValue() As Double
        Return _value * _linkedNeuron.value

        'There was slight debate regarding multiplying the weight value by 'value' or by 'calculateNeuron'.
        'The reason to use (_value*_linkedNeuron.value) and not (_value*_linkedNeuron.calculateNeuron) to calculate
        'the output value for this weight was because using 'value' is an O(n) calculation, although it requires more logic
        'configuration in the actual network object, while although using 'calculateNeuron' would only require calling the output neuron
        'to find the output for the entire network, it would actually be an O(n!) calculation because each node would actually
        'recalculate all the nodes beneath it, even if its neighbor nodes had done so already.

    End Function

End Class
