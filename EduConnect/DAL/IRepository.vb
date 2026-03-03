Imports System.Collections.Generic

Namespace DAL
    Public Interface IRepository(Of T)
        Function GetById(id As Integer) As T
        Function GetAll() As IEnumerable(Of T)
        Sub Add(entity As T)
        Sub Update(entity As T)
        Sub Delete(id As Integer)
    End Interface
End Namespace
