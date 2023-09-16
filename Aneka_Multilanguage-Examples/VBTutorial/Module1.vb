Imports System
Imports System.Collections.Generic
Imports Aneka
Imports Aneka.Threading
Imports Aneka.Entity
Imports System.Threading


Module Module1

    <Serializable()>
    Public Class HelloWorld
        Public result As String

        Public Function PrintHello()
            Console.WriteLine("Hello World!")
            result = "Hello from Visual Basic"
            Return True
        End Function

    End Class

    Sub Main()
        Dim app As AnekaApplication(Of AnekaThread, ThreadManager)

        Try
            Logger.Start()
            Dim conf As Configuration = Configuration.GetConfiguration("C:/Aneka/conf.xml")
            app = New AnekaApplication(Of AnekaThread, ThreadManager)(conf)

            Dim hw As HelloWorld = New HelloWorld()
            Dim th As AnekaThread = New AnekaThread(hw.PrintHello(), app)
            th.Start()
            th.Join()
            hw = th.Target
            Console.WriteLine("Value = " + hw.result)
        Catch ex As Exception

        End Try
        app.StopExecution()
        Logger.Stop()

    End Sub

End Module
