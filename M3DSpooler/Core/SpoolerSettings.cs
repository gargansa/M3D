﻿using M3D.Spooling.Common.Utils;
using M3D.Spooling.Core;
using System;
using System.IO;
using System.Security;
using System.Xml.Serialization;

namespace M3D.Spooler.Core
{
  public class SpoolerSettings
  {
    private static object threadsync = new object();
    private static XmlSerializer __class_serializer = null;
    public bool DoNotShowPrinterLockOutWarning;
    public bool StartAdvanced;

    public static SpoolerSettings LoadSettings()
    {
      var path = Path.Combine(Paths.SpoolerStorage, "settings.db3");
      var spoolerSettings = (SpoolerSettings) null;
      var textReader = (TextReader) null;
      lock (SpoolerSettings.threadsync)
      {
        try
        {
          textReader = new StreamReader(path);
          spoolerSettings = (SpoolerSettings) SpoolerSettings.ClassSerializer.Deserialize(textReader);
        }
        catch (Exception ex)
        {
        }
        finally
        {
          textReader?.Close();
        }
      }
      return spoolerSettings;
    }

    public static bool SaveSettings(SpoolerSettings settings, bool bShuttingDownSoCatchAllExceptions)
    {
      var str = Path.Combine(Paths.SpoolerStorage, "settings.db3");
      var flag = true;
      var textWriter1 = (TextWriter) null;
      lock (SpoolerSettings.threadsync)
      {
        try
        {
          var serializerNamespaces = new XmlSerializerNamespaces();
          serializerNamespaces.Add(string.Empty, string.Empty);
          XmlSerializer classSerializer = SpoolerSettings.ClassSerializer;
          textWriter1 = new StreamWriter(str);
          TextWriter textWriter2 = textWriter1;
          SpoolerSettings spoolerSettings = settings;
          XmlSerializerNamespaces namespaces = serializerNamespaces;
          classSerializer.Serialize(textWriter2, spoolerSettings, namespaces);
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is IOException || ex is InvalidOperationException)
        {
          flag = false;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException || (ex is ArgumentException || ex is ArgumentNullException) || ex is PathTooLongException)
        {
          if (bShuttingDownSoCatchAllExceptions)
          {
            flag = false;
          }
          else
          {
            throw;
          }
        }
        finally
        {
          if (textWriter1 != null)
          {
            textWriter1.Close();
            FileUtils.GrantAccess(str);
          }
        }
      }
      return flag;
    }

    public static XmlSerializer ClassSerializer
    {
      get
      {
        if (SpoolerSettings.__class_serializer == null)
        {
          SpoolerSettings.__class_serializer = new XmlSerializer(typeof (SpoolerSettings));
        }

        return SpoolerSettings.__class_serializer;
      }
    }
  }
}
