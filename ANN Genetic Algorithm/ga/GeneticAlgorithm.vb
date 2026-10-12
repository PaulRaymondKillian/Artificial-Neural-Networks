Imports System.IO

Public Class Chromosome319

    Private _result As Integer
    Private _weights(319) As Double
    Private _selected As Boolean

    Public Sub New()

        _result = 0
        _selected = False

        Dim tempRand As Double
        For i As Integer = 0 To 318
            tempRand = (RandomNumber() / (RandomNumber() + 1)) Mod 3

            If RandomNumber() > 50 Then
                tempRand = -tempRand
            End If

            _weights(i) = tempRand

            If _weights(i) > 30 Then
                MsgBox("You Should Never Get Here - Chromosome Constructor: _weight(" & i & ") = " & _weights(i))
            End If
        Next


    End Sub

    Public Property result() As Integer
        Get
            Return _result
        End Get
        Set(ByVal Value As Integer)
            _result = Value
        End Set
    End Property

    Public Property selected() As Boolean
        Get
            Return _selected
        End Get
        Set(ByVal Value As Boolean)
            _selected = Value
        End Set
    End Property

    Public Sub setWeight(ByVal i As Integer, ByVal v As Double)
        _weights(i) = v
    End Sub

    Public Function getWeight(ByVal i As Integer) As Double
        Return _weights(i)
    End Function

    Public Function getWeights() As Double()
        Return _weights
    End Function

    Public Function RandomNumber() As Integer
        Randomize(System.DateTime.Now.Millisecond)
        RandomNumber = Int(Rnd() * 100)
    End Function

End Class










Public Class Genetic_Algorithm_Controller

    Private _populationCount As Integer
    Private _chromosomeWeightCount As Integer
    Private _generations As Integer
    Private _population(100) As Chromosome319

    Private _bestResult As String

    Public Sub New()
        For i As Integer = 0 To 99
            _population(i) = New Chromosome319
        Next

        _generations = 0
        _populationCount = 100
        _chromosomeWeightCount = 319

    End Sub

    Public Function getWeight(ByVal i As Integer, ByVal j As Integer) As Double

        If (i < _populationCount And j < _chromosomeWeightCount) Then
            Return _population(i).getWeight(j)
        Else
            MsgBox("You Should Never Reach This Point - Function: getweight  Class: GeneticAlgorithm")
        End If
    End Function

    Public Function getWeights(ByVal i As Integer) As Double()
        Return _population(i).getWeights
    End Function

    Public Function getResult(ByVal i As Integer) As Integer
        Return _population(i).result
    End Function

    Public Sub setResult(ByVal i As Integer, ByVal j As Integer)
        _population(i).result = j
    End Sub

    Public Function wasSelected(ByVal i As Integer) As Boolean
        Return _population(i).selected
    End Function

    Public Sub sortPopulation()
        Dim sortedPopulation(100) As Chromosome319
        Dim newPopulation(100) As Chromosome319
        Dim currentBestResult As Integer = -1
        Dim currentBestIndex As Integer = -1

        'Sort the list of Chromosomes into sortedPopulation
        For i As Integer = 0 To 99
            For j As Integer = 0 To 99
                If getResult(j) > currentBestResult And wasSelected(j) = False Then
                    currentBestResult = getResult(j)
                    currentBestIndex = j
                End If
            Next

            newPopulation(i) = _population(currentBestIndex)
            _population(currentBestIndex).selected = True
            currentBestResult = -1

        Next
        'Done Sorting


        _population = newPopulation

        _bestResult = _population(0).result.ToString

    End Sub

    Public Function crossBreed(ByVal a As Chromosome319, ByVal b As Chromosome319)
        Dim tempChromosome As New Chromosome319
        Dim point As Integer = (RandomNumber() * RandomNumber()) Mod 318

        For i As Integer = 0 To point
            tempChromosome.setWeight(i, a.getWeight(i))
        Next

        For i As Integer = point + 1 To 318
            tempChromosome.setWeight(i, b.getWeight(i))
        Next

        Return tempChromosome
    End Function

    Public Sub nextGeneration()
        Dim newPopulation(100) As Chromosome319
        Dim mutationPercentage As Integer = 3


        'First sort the population with the best first
        sortPopulation()
        debugOutputPopulationData()

        'Clone the top 30, so we leave them entirely alone and continue
        For i As Integer = 0 To 29
            newPopulation(i) = _population(i)
        Next


        'Cross Breed the best 30 together
        For i As Integer = 30 To 59
            newPopulation(i) = crossBreed(_population(RandomNumber() Mod 30), _population(RandomNumber() Mod 30))
        Next



        'Cross Breed all together (no class restrictions)
        For i As Integer = 60 To 89
            newPopulation(i) = crossBreed(_population(RandomNumber() Mod 100), _population(RandomNumber() Mod 100))
        Next

        'Randomly produce last 25 chromosomes
        For i As Integer = 90 To 99
            Dim tempChromosome As New Chromosome319
            newPopulation(i) = tempChromosome
        Next


        'Allow a small number of mutations to occur, except the top 5
        For i As Integer = 5 To 99
            If (RandomNumber() < mutationPercentage) Then
                Dim rand As Double = (RandomNumber() / (RandomNumber() + 1))   'Mutation will always be less than 1
                Dim weightIndex = (RandomNumber() * RandomNumber() Mod 319)     'Index of weight to be mutated

                If RandomNumber() > 50 Then 'Either + or -
                    rand = -rand
                End If

                MsgBox("Mutation Value: " & rand)

                newPopulation(i).setWeight(weightIndex, newPopulation(i).getWeight(weightIndex) + rand)

            End If
        Next

        _population = newPopulation

    End Sub

    Public Function getBest() As Integer
        Return _bestResult
    End Function

    Public Sub debugOutputPopulationData()
        Dim sr As StreamWriter
        sr = New StreamWriter("C:\PopulationData.txt")

        For i As Integer = 0 To 99
            sr.WriteLine("Chromosome: " & i & " Result: " & _population(i).result)
        Next
        sr.Close()
    End Sub

    Public Function RandomNumber() As Integer
        Randomize(System.DateTime.Now.Millisecond)
        RandomNumber = Int(Rnd() * 100)
    End Function

End Class

