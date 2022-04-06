Imports System
Imports System.Linq

Namespace Issues
    Public Module IssuesContextInitializer
        Public Sub Seed()
            Dim context = New IssuesContext()
            Dim users = OutlookDataGenerator.Users.[Select](Function(x)
                                                                Dim split = x.Split(" "c)
                                                                Return New User() With {
                                                                    .FirstName = split(0),
                                                                    .LastName = split(1)
                                                                }
                                                            End Function).ToArray()
            context.Users.AddRange(users)
            context.SaveChanges()
            Dim rnd = New Random(0)
            Dim issues = Enumerable.Range(0, 1000).[Select](Function(i) New Issue() With {
                .Subject = OutlookDataGenerator.GetSubject(),
                .UserId = users(rnd.Next(users.Length)).Id,
                .Created = Date.Today.AddDays(-rnd.Next(30)),
                .Priority = OutlookDataGenerator.GetPriority(),
                .Votes = rnd.Next(100)
            }).ToArray()
            context.Issues.AddRange(issues)
            context.SaveChanges()
        End Sub
    End Module
End Namespace
