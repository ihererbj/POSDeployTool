Imports Microsoft.VisualBasic.ApplicationServices
Imports POSDeployTool.Helpers
Imports POSDeployTool.Infrastructure

Namespace My
    Partial Friend Class MyApplication
        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
            AddHandler Application.ThreadException, AddressOf OnUiThreadException
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf OnDomainUnhandledException
            AddHandler TaskScheduler.UnobservedTaskException, AddressOf OnUnobservedTaskException
            AppServices.Current.Audit.Write("APPLICATION_START", Environment.MachineName, "SUCCESS")
        End Sub

        Private Sub MyApplication_Shutdown(sender As Object, e As EventArgs) Handles Me.Shutdown
            AppServices.Current.Audit.Write("APPLICATION_STOP", Environment.MachineName, "SUCCESS")
            AppServices.Current.Logger.Info("Application stopped.")
        End Sub

        Private Shared Sub OnUiThreadException(sender As Object, e As Threading.ThreadExceptionEventArgs)
            HandleFatalException("UI thread exception", e.Exception)
        End Sub

        Private Shared Sub OnDomainUnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
            Dim ex = TryCast(e.ExceptionObject, Exception)
            HandleFatalException("Unhandled application exception", ex)
        End Sub

        Private Shared Sub OnUnobservedTaskException(sender As Object, e As UnobservedTaskExceptionEventArgs)
            AppServices.Current.Logger.Error("Unobserved task exception", e.Exception)
            e.SetObserved()
        End Sub

        Private Shared Sub HandleFatalException(message As String, ex As Exception)
            Try
                AppServices.Current.Logger.Critical(message, ex)
                AppServices.Current.Audit.Write("APPLICATION_ERROR", Environment.MachineName, "FAILED", If(ex Is Nothing, message, ex.Message))
            Catch
            End Try

            MessageBox.Show(message & Environment.NewLine & Environment.NewLine & If(ex Is Nothing, "Unknown error", ex.Message) &
                            Environment.NewLine & Environment.NewLine & "ตรวจสอบรายละเอียดที่โฟลเดอร์ Logs",
                            "POS Deploy Tool",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Sub
    End Class
End Namespace
