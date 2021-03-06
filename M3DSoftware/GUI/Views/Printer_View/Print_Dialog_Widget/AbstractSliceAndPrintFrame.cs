﻿using M3D.GUI.Controller.Settings;
using M3D.GUI.Tools;
using M3D.GUI.Views.Library_View;
using M3D.Slicer.General;
using M3D.Spooling.Client;
using M3D.Spooling.Common;
using System.Collections.Generic;
using System.IO;

namespace M3D.GUI.Views.Printer_View.Print_Dialog_Widget
{
  internal abstract class AbstractSliceAndPrintFrame : IPrintDialogFrame
  {
    private ThreadSafeVariable<bool> _slicer_started = new ThreadSafeVariable<bool>(false);

    public AbstractSliceAndPrintFrame(int ID, PrintDialogMainWindow printDialogWindow)
      : base(ID, printDialogWindow)
    {
      ResetSlicerState();
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
    }

    public string ProcessNextSlicerMessage()
    {
      var messageFromQueue = SlicerConnection.GetMessageFromQueue();
      if (messageFromQueue == "Slicer Started")
      {
        BHasSlicerStarted = true;
      }
      else if (messageFromQueue == "Slicer Finished")
      {
        BHasSlicerStarted = false;
        BHasSlicingCompleted = true;
      }
      return messageFromQueue;
    }

    public void ResetSlicerState()
    {
      BHasSlicerStarted = false;
      BHasSlicingCompleted = false;
    }

    public void StartSlicer(M3D.Slicer.General.PrintSettings printsettings)
    {
      var num = (int)SlicerConnection.StartSlicingUsingCurrentSettings(Paths.CombinedSTLPath, "m3doutput.gcode", printsettings);
    }

    public void PrintSlicedModel(PrintJobDetails currentJobDetails, RecentPrintsTab recentPrints, AsyncCallback OnPrintJobStarted)
    {
      var gcodefile = Path.Combine(Paths.WorkingFolder, "m3doutput.gcode");
      var filepath = "M3D.M3D";
      foreach (PrintDetails.ObjectDetails objectDetails in currentJobDetails.objectDetailsList)
      {
        if (currentJobDetails.autoPrint)
        {
          objectDetails.hidecontrols = true;
        }

        filepath = objectDetails.filename;
      }
      var splitFileName = new SplitFileName(filepath);
      var jobParams = new JobParams(gcodefile, splitFileName.name + "." + splitFileName.ext, currentJobDetails.preview_image, FilamentSpool.TypeEnum.OtherOrUnknown, (int)currentJobDetails.Estimated_Print_Time, currentJobDetails.Estimated_Filament)
      {
        options = currentJobDetails.jobOptions,
        preprocessor = currentJobDetails.printer.MyFilamentProfile.preprocessor,
        filament_temperature = currentJobDetails.printer.MyFilamentProfile.Temperature,
        autoprint = currentJobDetails.autoPrint
      };
      List<Slicer.General.KeyValuePair<string, string>> keyValuePairList = SlicerSettings.GenerateUserKeyValuePairList();
      SettingsManager.SavePrintingObjectsDetails(jobParams, currentJobDetails.objectDetailsList);
      SettingsManager.SavePrintJobInfo(jobParams, currentJobDetails.printer, SlicerSettings.ProfileName, SlicerSettings.GenerateUserKeyValuePairList());
      recentPrints?.AddRecentPrintHistory(jobParams, currentJobDetails.printer, SlicerSettings.ProfileName, keyValuePairList, currentJobDetails.objectDetailsList);
      if (currentJobDetails.print_to_file)
      {
        jobParams.outputfile = currentJobDetails.printToFileOutputFile;
        jobParams.jobMode = JobParams.Mode.SaveToBinaryGCodeFile;
      }
      else if (currentJobDetails.auto_untethered_print)
      {
        jobParams.jobMode = !currentJobDetails.sdSaveOnly_print ? JobParams.Mode.SavingToSDCardAutoStartPrint : JobParams.Mode.SavingToSDCard;
      }

      var num = (int) currentJobDetails.printer.PrintModel(OnPrintJobStarted, currentJobDetails.printer, jobParams);
    }

    protected bool BHasSlicingCompleted { get; private set; }

    protected bool BHasSlicerStarted
    {
      get
      {
        return _slicer_started.Value;
      }
      private set
      {
        _slicer_started.Value = value;
      }
    }
  }
}
