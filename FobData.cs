using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ekin.EACFOB
{
    public class FobData
    {
        public Vector3 Location;

        public ushort InstanceID;

        public string Name;

        public string Owner;

        public string Type;

        public FobData(Vector3 location, ushort instanceID, string name, string owner, string type)
        {
            Location = location;
            InstanceID = instanceID;
            Name = name;
            Owner = owner;
            Type = type;
        }
    }
}
