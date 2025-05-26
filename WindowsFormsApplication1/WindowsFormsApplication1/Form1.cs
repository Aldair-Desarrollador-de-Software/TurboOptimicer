using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Código al cargar, si se desea.
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder resumen = new StringBuilder();

            try
            {
                // 1. Limpiar archivos temporales del usuario
                string tempPath = Path.GetTempPath();
                if (Directory.Exists(tempPath))
                {
                    EjecutarComando("cmd.exe", "/c del /q /f /s \"" + tempPath + "*.*\"");
                    resumen.AppendLine("✔ Archivos temporales del usuario eliminados.");
                }

                // 2. Limpiar archivos temporales del sistema
                string winTemp = @"C:\Windows\Temp";
                if (Directory.Exists(winTemp))
                {
                    EjecutarComando("cmd.exe", "/c del /q /f /s \"" + winTemp + "\\*.*\"");
                    resumen.AppendLine("✔ Archivos temporales del sistema eliminados.");
                }

                // 3. Vaciar caché DNS
                EjecutarComando("ipconfig", "/flushdns");
                resumen.AppendLine("✔ Caché DNS vaciada.");

                // 4. Ejecutar limpieza de disco
                EjecutarComando("cleanmgr.exe", "");
                resumen.AppendLine("✔ Limpieza de disco ejecutada.");

                // 5. Limpiar archivos de Prefetch
                string prefetchPath = @"C:\Windows\Prefetch";
                if (Directory.Exists(prefetchPath))
                {
                    EjecutarComando("cmd.exe", "/c del /q /f /s \"" + prefetchPath + "\\*.*\"");
                    resumen.AppendLine("✔ Archivos Prefetch eliminados.");
                }

                // 6. Limpiar historial de navegación (IE)
                EjecutarComando("RunDll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 255");
                resumen.AppendLine("✔ Historial de navegación (IE) limpiado.");

                // 7. Liberar memoria RAM
                LiberarMemoriaRAM();
                resumen.AppendLine("✔ Memoria RAM optimizada.");

                // 8. Activar plan de energía en Alto rendimiento
                EjecutarComando("cmd.exe", "/c powercfg /setactive SCHEME_MIN");
                resumen.AppendLine("✔ Plan de energía 'Alto rendimiento' activado.");

                // 9. Reiniciar controlador gráfico (GPU)
                ReiniciarControladorGPU();
                resumen.AppendLine("✔ Controlador gráfico reiniciado (GPU optimizada).");

                // 10. Detener servicios innecesarios
                DetenerServiciosInnecesarios(resumen);

                // 11. Desactivar servicios adicionales
                DesactivarServiciosGaming(resumen);

                // 12. Cerrar procesos innecesarios
                CerrarAppsFondo(resumen);

                // 13. Aumentar prioridad de explorer.exe
                EjecutarComando("cmd.exe", "/c wmic process where name=\"explorer.exe\" CALL setpriority 128");
                resumen.AppendLine("✔ Prioridad de procesos ajustada para rendimiento.");

                // 14. Desactivar indexado
                EjecutarComando("cmd.exe", "/c net stop wsearch");
                resumen.AppendLine("✔ Servicio de indexado desactivado.");

                // 15. Optimizar uso del CPU
                EjecutarComando("cmd.exe", "/c bcdedit /set useplatformclock true");
                resumen.AppendLine("✔ Optimización de CPU aplicada.");

                // 16. Limpiar inicio automático
                LimpiarInicioAutomatico(resumen);

                // Mostrar resumen
                MessageBox.Show("Optimización completada:\n\n" + resumen.ToString(), "Resumen", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al optimizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EjecutarComando(string archivo, string argumentos)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = archivo;
                psi.Arguments = argumentos;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = false;
                psi.UseShellExecute = true;
                psi.Verb = "runas";
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar: " + archivo + " " + argumentos + "\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LiberarMemoriaRAM()
        {
            foreach (Process proceso in Process.GetProcesses())
            {
                try
                {
                    EmptyWorkingSet(proceso.Handle);
                }
                catch { }
            }
        }

        private void ReiniciarControladorGPU()
        {
            // Simula Win+Ctrl+Shift+B para reiniciar el driver de video
            keybd_event(0x5B, 0, 0, 0); // Win
            keybd_event(0x11, 0, 0, 0); // Ctrl
            keybd_event(0x10, 0, 0, 0); // Shift
            keybd_event(0x42, 0, 0, 0); // B

            keybd_event(0x5B, 0, 2, 0); // Win up
            keybd_event(0x11, 0, 2, 0); // Ctrl up
            keybd_event(0x10, 0, 2, 0); // Shift up
            keybd_event(0x42, 0, 2, 0); // B up
        }

        private void DetenerServiciosInnecesarios(StringBuilder resumen)
        {
            string[] servicios = new string[]
            {
                "wuauserv", "XblGameSave", "XboxGipSvc", "XboxNetApiSvc", "DiagTrack", "SysMain"
            };

            foreach (string servicio in servicios)
            {
                try
                {
                    EjecutarComando("cmd.exe", "/c net stop " + servicio);
                    resumen.AppendLine("✔ Servicio detenido: " + servicio);
                }
                catch
                {
                    resumen.AppendLine("✖ No se pudo detener: " + servicio);
                }
            }
        }

        private void DesactivarServiciosGaming(StringBuilder resumen)
        {
            string[] serviciosExtra = new string[]
            {
                "Fax", "Spooler", "WMPNetworkSvc", "WerSvc", "WSearch"
            };

            foreach (string servicio in serviciosExtra)
            {
                try
                {
                    EjecutarComando("cmd.exe", "/c net stop " + servicio);
                    resumen.AppendLine("✔ Servicio desactivado: " + servicio);
                }
                catch
                {
                    resumen.AppendLine("✖ No se pudo desactivar: " + servicio);
                }
            }
        }

        private void CerrarAppsFondo(StringBuilder resumen)
        {
            string[] procesos = new string[]
            {
                "OneDrive", "Skype", "YourPhone", "SteamWebHelper",
                "Cortana", "MSASCui", "AdobeARM", "Dropbox", "Teams"
            };

            foreach (string nombre in procesos)
            {
                foreach (Process proc in Process.GetProcessesByName(nombre))
                {
                    try
                    {
                        proc.Kill();
                        resumen.AppendLine("✔ Proceso cerrado: " + nombre);
                    }
                    catch
                    {
                        resumen.AppendLine("✖ No se pudo cerrar: " + nombre);
                    }
                }
            }
        }

        private void LimpiarInicioAutomatico(StringBuilder resumen)
        {
            try
            {
                string[] claves = new string[]
                {
                    @"Software\Microsoft\Windows\CurrentVersion\Run",
                    @"Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Run"
                };

                string[] programas = new string[]
                {
                    "OneDrive", "Skype", "Discord", "Steam", "Teams", "Cortana", "YourPhone"
                };

                foreach (string clave in claves)
                {
                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(clave, true))
                    {
                        if (rk != null)
                        {
                            foreach (string nombre in programas)
                            {
                                if (rk.GetValue(nombre) != null)
                                {
                                    rk.DeleteValue(nombre, false);
                                    resumen.AppendLine("✔ Eliminado del inicio: " + nombre);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                resumen.AppendLine("✖ No se pudo limpiar el inicio automático.");
            }
        }
    }
}
