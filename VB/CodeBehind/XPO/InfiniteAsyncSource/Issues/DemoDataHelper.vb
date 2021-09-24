Imports DevExpress.Xpo
Imports System
Imports System.Linq

Namespace Issues
    Public Module DemoDataHelper
        Public Sub Seed()
            Using uow = New DevExpress.Xpo.UnitOfWork()
                Dim users = OutlookDataGenerator.Users.[Select](Function(x)
                                                                    Dim split = x.Split(" "c)
                                                                    Return New User(uow) With {
                                                                        .FirstName = split(0),
                                                                        .LastName = split(1)
                                                                    }
                                                                End Function).ToArray()
                uow.CommitChanges()
                Dim rnd = New Random(0)
                Dim issues = Enumerable.Range(0, 1000).[Select](Function(i) New Issue(uow) With {
                    .Subject = OutlookDataGenerator.GetSubject(),
                    .UserId = users(rnd.Next(users.Length)).Oid,
                    .Created = Date.Today.AddDays(-rnd.Next(30)),
                    .Priority = OutlookDataGenerator.GetPriority(),
                    .Votes = rnd.Next(100)
                }).ToArray()
                uow.CommitChanges()
            End Using
        End Sub
    End Module
End Namespace

