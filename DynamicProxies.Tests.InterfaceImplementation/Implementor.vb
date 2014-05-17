Public Class Implementor
	Implements IOne, ITwo, IThree, IFour

	Public Sub Calling() Implements IOne.Invoke, ITwo.Invoke, IThree.CallIt

	End Sub

	Public Sub Invoke() Implements IFour.Invoke

	End Sub
End Class
