﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace M3D.Spooling.Common
{
  public class StackedBoundingBox
  {
    [XmlElement]
    public List<BoundingBox> bounds_list;
    [XmlAttribute]
    public float min_z;
    [XmlAttribute]
    public float max_z;

    public StackedBoundingBox()
    {
      bounds_list = new List<BoundingBox>();
    }

    public StackedBoundingBox(StackedBoundingBox other)
    {
      bounds_list = new List<BoundingBox>(other.bounds_list);
      min_z = other.min_z;
      max_z = other.max_z;
    }

    public StackedBoundingBox(params BoundingBox[] regions)
    {
      if (regions == null || regions.Length < 1)
      {
        throw new ArgumentException("Must include at least one region.");
      }

      bounds_list = new List<BoundingBox>(regions);
      min_z = bounds_list[0].min.z;
      max_z = bounds_list[0].max.z;
      foreach (BoundingBox bounds in bounds_list)
      {
        if (bounds.max.z < (double)max_z)
        {
          throw new ArgumentException("Bounding boxes must be in order from Z min to Z max.");
        }

        max_z = bounds.max.z;
      }
    }

    public bool OverLap(BoundingBox other)
    {
      foreach (BoundingBox bounds in bounds_list)
      {
        if (other.OverLap(bounds))
        {
          return true;
        }
      }
      return false;
    }

    public bool InRegion(Vector3D p)
    {
      foreach (BoundingBox bounds in bounds_list)
      {
        if (bounds.InRegion(p))
        {
          return true;
        }
      }
      return false;
    }

    public bool InRegionNaN(Vector3D p)
    {
      foreach (BoundingBox bounds in bounds_list)
      {
        if (bounds.InRegionNaN(p))
        {
          return true;
        }
      }
      return false;
    }

    public bool LineIntercepts(out Vector3D intercept, Vector3D p1, Vector3D p2)
    {
      for (var index1 = 0; index1 < bounds_list.Count; ++index1)
      {
        if (bounds_list[index1].LineIntercepts(out intercept, p1, p2))
        {
          var flag = false;
          for (var index2 = 0; index2 < bounds_list.Count; ++index2)
          {
            if (index2 != index1)
            {
              flag |= bounds_list[index2].InRegion(intercept);
            }
          }
          if (!flag)
          {
            return true;
          }
        }
      }
      intercept = p2;
      return false;
    }

    public int OutOfBoundsCheck(float X, float Y, float Z)
    {
      if (Z < (double)min_z)
      {
        return 16;
      }

      if (Z > (double)max_z)
      {
        return 32;
      }

      foreach (BoundingBox bounds in bounds_list)
      {
        if (Z <= (double)bounds.max.z && Z >= (double)bounds.min.z)
        {
          return bounds.OutOfBoundsCheck(X, Y, Z);
        }
      }
      return 0;
    }

    public override bool Equals(object obj)
    {
      StackedBoundingBox stackedBoundingBox = this;
      var other = obj as StackedBoundingBox;
      if ((object) stackedBoundingBox == null && (object) other == null)
      {
        return true;
      }

      if ((object) stackedBoundingBox == null || (object) other == null)
      {
        return false;
      }

      return stackedBoundingBox.Equals(other);
    }

    public bool Equals(StackedBoundingBox other)
    {
      if (bounds_list.Count != other.bounds_list.Count || min_z != (double)other.min_z || max_z != (double)other.max_z)
      {
        return false;
      }

      for (var index = 0; index < bounds_list.Count; ++index)
      {
        if (bounds_list[index] != other.bounds_list[index])
        {
          return false;
        }
      }
      return true;
    }

    public override int GetHashCode()
    {
      var num = min_z.GetHashCode() ^ max_z.GetHashCode();
      for (var index = 0; index < bounds_list.Count; ++index)
      {
        num ^= bounds_list[index].GetHashCode();
      }

      return num;
    }

    public static bool operator ==(StackedBoundingBox a, StackedBoundingBox b)
    {
      if ((object) a == null && (object) b == null)
      {
        return true;
      }

      if ((object) a == null || (object) b == null)
      {
        return false;
      }

      return a.Equals(b);
    }

    public static bool operator !=(StackedBoundingBox a, StackedBoundingBox b)
    {
      return !a.Equals(b);
    }
  }
}
