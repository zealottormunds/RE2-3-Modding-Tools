using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using OpenTK;
//using OpenTK.Input;
//using OpenTK.Graphics;
//using SmoothGL.Graphics;

namespace Resident_Evil_2_Modding_Tools._3D
{
    /*class zVector3
    {
        float x = 0;
        float y = 0;
        float z = 0;

        float rotx = 0;

        public zVector3(float v1, float v2, float v3)
        {
            this.x = v1;
            this.y = v2;
            this.z = v3;
        }

        public zVector3(Vector3 a)
        {
            this.x = a.X;
            this.y = a.Y;
            this.z = a.Z;
        }

        public Vector3 convert(zVector3 a)
        {
            Vector3 n = new Vector3();

            n.X = a.x;
            n.Y = a.y;
            n.Z = a.z;

            return n;
        }

        float Distance(zVector3 a, zVector3 b)
        {
            float d = (float)Math.Sqrt(
                Math.Pow(b.x - a.x, 2) +
                Math.Pow(b.y - a.y, 2) +
                Math.Pow(b.z - a.z, 2)
                );

            return d;
        }

        zVector3 zero()
        {
            return new zVector3(0, 0, 0);
        }

        zVector3 one()
        {
            return new zVector3(0, 0, 0);
        }

        zVector3 add(zVector3 a, zVector3 b)
        {
            zVector3 c = zero();
            c.x = a.x + b.x;
            c.y = a.y + b.y;
            c.z = a.z + b.z;
            return c;
        }

        zVector3 sub(zVector3 a, zVector3 b)
        {
            zVector3 c = zero();
            c.x = a.x - b.x;
            c.y = a.y - b.y;
            c.z = a.z - b.z;
            return c;
        }

        bool cmp(zVector3 a, zVector3 b)
        {
            bool equal = false;

            if (a.x == b.x && a.y == b.y && a.z == b.z) equal = true;

            return equal;
        }

        zVector3 mag(zVector3 a, float b)
        {
            return new zVector3(a.x * b, a.y * b, a.z * b);
        }

        int RoundDown(int toRound)
        {
            return toRound - toRound % 100;
        }

        float magnitude(zVector3 a)
        {
            if (a.x == 0 && a.y == 0 && a.z == 0) return 0;

            float mag = (float)Math.Sqrt(
                Math.Pow(a.x, 2) + Math.Pow(a.y, 2) + Math.Pow(a.z, 2)
            );

            return mag;
        }

        zVector3 normalize(zVector3 a)
        {
            float mag = magnitude(a);

            zVector3 normalized = new zVector3(a.x / mag, a.y / mag, a.z / mag);
            return normalized;
        }

        zVector3 lerp(zVector3 a, zVector3 b, float m)
        {
            float addtox = b.x - a.x;
            float addtoy = b.y - a.y;
            float addtoz = b.z - a.z;

            return new zVector3
            (
                a.x + (addtox * m),
                a.y + (addtoy * m),
                a.z + (addtoz * m)
            );
        }

        zVector3 forward()
        {
            float x = (float)(10 * Math.Cos(rotx));
            float y = (float)(10 * Math.Sin(rotx));
            float z = 0;

            return new zVector3(x, y, z);
        }

        zVector3 right()
        {
            float x = (float)(10 * Math.Sin(rotx));
            float y = -(float)(10 * Math.Cos(rotx));
            float z = 0;

            return new zVector3(x, y, z);
        }

        zVector3 up()
        {
            float x = 0;
            float y = 0;
            float z = 10;

            return new zVector3(x, y, z);
        }
    }*/
}
