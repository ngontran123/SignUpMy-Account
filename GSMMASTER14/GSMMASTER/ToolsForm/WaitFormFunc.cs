using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSMMASTER.ToolsForm
{
    public class WaitFormFunc
    {
        WaingForm wait;
        public void Load()
        {
            Task.Run(()=>{ loadingProcess(); });
        }
        public void Load(SendSmS parent)
        {
          Task.Run(() => { loadingProcess(parent); });
        }
        public void Close()
        {
            if(wait!= null) {
           wait.closeWaitForm();
            }
        }
        public void loadingProcess()
        {
            wait = new WaingForm();
            wait.ShowDialog();
        }
        public void loadingProcess(SendSmS parent)
        {
            wait = new WaingForm(parent);
            wait.ShowDialog();
        }
    }
}
