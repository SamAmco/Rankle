using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;
using FarseerPhysics.DemoBaseXNA;

namespace GameFinal
{
    class StaticHelpers
    {
        public static float fontScale { get { return 0.25f; } private set { } }
        public static bool debugMode { get { return false; } set { } }

        /// <summary>
        /// This will give you a normalized vector rotated clockwise by the
        /// number of radians you pass to it from north. It can be used to get
        /// velocities of bullets ect :)
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector2 VectorFromRotation(float rotation)
        {
            Vector2 vector = new Vector2(0, -1);
            rotation = WrapAngle(rotation);

            Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
            vector = Vector2.Transform(vector, rotationMatrix);
            return vector;            
        }

        public static float RotationFromVector(Vector2 vector)
        {
            float rotation = 0;
            vector.Normalize();
            rotation = (float)Math.Atan2(-vector.Y, vector.X);
            rotation -= MathHelper.PiOver2;
            rotation = -rotation;
            return rotation;
        }

        public static Vector2 RotateVector(Vector2 vector, float rotation)
        {
            rotation = WrapAngle(rotation);

            Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
            vector = Vector2.Transform(vector, rotationMatrix);

            return vector;
        }
        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        public static float TurnToFace(Vector2 position, Vector2 faceThis)
        {
            // consider this diagram:
            //         C 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // S--------
            //     x
            // 
            // where S is the position of the spot light, C is the position of the cat,
            // and "o" is the angle that the spot light should be facing in order to 
            // point at the cat. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            float desiredAngle = (float)Math.Atan2(y, x);

            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            //float difference = WrapAngle(desiredAngle - currentAngle);

            //// clamp that between -turnSpeed and turnSpeed.
            //difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            //return WrapAngle(currentAngle + difference);
            return WrapAngle(desiredAngle);
        }
        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public static float WrapToFullCircle(float radians)
        {
            //Console.WriteLine(MathHelper.ToDegrees(radians));
            radians = radians % MathHelper.TwoPi;


            if (radians < 0)
                radians = MathHelper.TwoPi + radians;

            //Console.WriteLine(MathHelper.ToDegrees(radians));
            return radians;
        }

        public static List<Vector2> ShuffleVectors(List<Vector2> list)
        {
            Random rnd = new Random();
            List<Vector2> list2 = new List<Vector2>();
            foreach (Vector2 v in list)
            {
                list2.Add(Vector2.Zero);
            }
            for(int n = 0; n < list2.Count; n++)
            {
                int i = rnd.Next(0, list.Count);
                list2[n] = list[i];
                list.Remove(list[i]);
            }
            return list2;
        }

        public static bool RayCast(Vector2 pos1, Vector2 pos2, World world, Body excludeBody)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 currentPos = new Vector2(pos1.X + (pos2.X - pos1.X) * (Convert.ToSingle(i) / 3.0f), pos1.Y + (pos2.Y - pos1.Y) * (Convert.ToSingle(i) / 3.0f));
                List<Fixture> fixtures = world.TestPointAll(currentPos);
                for (int j = 0; j < fixtures.Count; j++)
                {
                    foreach (Fixture excludeFix in excludeBody.FixtureList)
                    {
                        if (fixtures[j] == excludeFix)
                        {
                            fixtures.RemoveAt(j);
                        }
                    }
                }
                if (fixtures.Count > 0)
                {
                    return true;
                }

            }
            return false;
        }

        public static String keyToLetter(Keys k, bool canBeLetter, bool shift)
        {
            if (canBeLetter)
            {
                switch (k)
                {
                    case Keys.A: return "a";
                    case Keys.B: return "b";
                    case Keys.C: return "c";
                    case Keys.D: return "d";
                    case Keys.E: return "e";
                    case Keys.F: return "f";
                    case Keys.G: return "g";
                    case Keys.H: return "h";
                    case Keys.I: return "i";
                    case Keys.J: return "j";
                    case Keys.K: return "k";
                    case Keys.L: return "l";
                    case Keys.M: return "m";
                    case Keys.N: return "n";
                    case Keys.O: return "o";
                    case Keys.P: return "p";
                    case Keys.Q: return "q";
                    case Keys.R: return "r";
                    case Keys.S: return "s";
                    case Keys.T: return "t";
                    case Keys.U: return "u";
                    case Keys.V: return "v";
                    case Keys.W: return "w";
                    case Keys.X: return "x";
                    case Keys.Y: return "y";
                    case Keys.Z: return "z";

                    case Keys.D0: return ((shift) ? ')' : '0').ToString();
                    case Keys.D1: return ((shift) ? '!' : '1').ToString();
                    case Keys.D2: return ((shift) ? '@' : '2').ToString();
                    case Keys.D3: return ((shift) ? '#' : '3').ToString();
                    case Keys.D4: return ((shift) ? '$' : '4').ToString();
                    case Keys.D5: return ((shift) ? '%' : '5').ToString();
                    case Keys.D6: return ((shift) ? '^' : '6').ToString();
                    case Keys.D7: return ((shift) ? '&' : '7').ToString();
                    case Keys.D8: return ((shift) ? '*' : '8').ToString();
                    case Keys.D9: return ((shift) ? '(' : '9').ToString();

                    case Keys.Add: return       ('+').ToString();
                    case Keys.Divide: return    ('/').ToString();
                    case Keys.Multiply: return  ('*').ToString();
                    case Keys.Subtract: return  ('-').ToString();
                    case Keys.Space: return " ";

                    case Keys.OemBackslash: return     (shift ? '|' : '\\').ToString();
                    case Keys.OemCloseBrackets: return (shift ? '}' : ']' ).ToString();
                    case Keys.OemComma: return         (shift ? '<' : ',' ).ToString();
                    case Keys.OemMinus: return         (shift ? '_' : '-' ).ToString();
                    case Keys.OemOpenBrackets: return  (shift ? '{' : '[' ).ToString();
                    case Keys.OemPeriod: return        (shift ? '>' : '.' ).ToString();
                    case Keys.OemPipe: return          (shift ? '|' : '\\').ToString();
                    case Keys.OemPlus: return          (shift ? '+' : '=' ).ToString();
                    case Keys.OemQuestion: return      (shift ? '?' : '/' ).ToString();
                    case Keys.OemQuotes: return        (shift ? '"' : '\'').ToString();
                    case Keys.OemSemicolon: return     (shift ? ':' : ';' ).ToString();
                    case Keys.OemTilde: return         (shift ? '~' : '`' ).ToString();
                    default: return "";
                }
            }
            else
            {
                switch (k)
                {
                    case Keys.D0:
                        return "0";
                    case Keys.D1:
                        return "1";
                    case Keys.D2:
                        return "2";
                    case Keys.D3:
                        return "3";
                    case Keys.D4:
                        return "4";
                    case Keys.D5:
                        return "5";
                    case Keys.D6:
                        return "6";
                    case Keys.D7:
                        return "7";
                    case Keys.D8:
                        return "8";
                    case Keys.D9:
                        return "9";
                    case Keys.Space:
                        return " ";
                    default: return "";
                }
            }
        }
        /// 640, 480        0
        /// 720, 480        1
        /// 720, 576        2
        /// 800, 600        3
        /// 1024, 768       4
        /// 1152, 864       5
        /// 1280, 720       6
        /// 1280, 768       7
        /// 1280, 800       8
        /// 1366, 768       9
        /// 1600, 900       10
        /// 1920, 1080      11
        public static int resolutionToIndex(int width, int height)
        {
            int index = 15;
            switch (height)
            {
                case 480:
                    if (width == 640)
                        return 0;
                    else return 1;
                case 576:
                    return 2;
                case 600:
                    return 3;
                case 720:
                    return 6;
                case 768:
                    if (width == 1024)
                        return 4;
                    else if (width == 1280)
                        return 6;
                    else return 9;
                case 800:
                    return 8;
                case 864:
                    return 5;
                case 900:
                    return 10;
                case 1080:
                    return 11;
            }
            return index;
        }

        public static int[] indexToResolution(int index)
        {
            int[] r = new int[2];
            switch (index)
            {
                case 0:
                    r[0] = 640;
                    r[1] = 480;
                    break;
                case 1:
                    r[0] = 720;
                    r[1] = 480;
                    break;
                case 2:
                    r[0] = 720;
                    r[1] = 576;
                    break;
                case 3:
                    r[0] = 800;
                    r[1] = 600;
                    break;
                case 4:
                    r[0] = 1024;
                    r[1] = 768;
                    break;
                case 5:
                    r[0] = 1152;
                    r[1] = 864;
                    break;
                case 6:
                    r[0] = 1280;
                    r[1] = 720;
                    break;
                case 7:
                    r[0] = 1280;
                    r[1] = 768;
                    break;
                case 8:
                    r[0] = 1280;
                    r[1] = 800;
                    break;
                case 9:
                    r[0] = 1366;
                    r[1] = 768;
                    break;
                case 10:
                    r[0] = 1600;
                    r[1] = 900;
                    break;
                case 11:
                    r[0] = 1920;
                    r[1] = 1080;
                    break;
            }
            return r;
        }

        public static bool RayCast(Vector2 pos1, Vector2 pos2, World world, Body excludeBody1, Body excludeBody2)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 currentPos = new Vector2(pos1.X + ((pos2.X - pos1.X) * (Convert.ToSingle(i) / 3.0f)), pos1.Y + ((pos2.Y - pos1.Y) * (Convert.ToSingle(i) / 3.0f)));
                List<Fixture> fixtures = world.TestPointAll(currentPos);
                for (int j = 0; j < fixtures.Count; j++)
                {
                    foreach (Fixture excludeFix in excludeBody1.FixtureList)
                    {
                        if (fixtures[j] == excludeFix)
                        {
                            fixtures.RemoveAt(j);
                        }
                    }
                    if (fixtures.Count > 0)
                    {
                        foreach (Fixture excludeFix in excludeBody2.FixtureList)
                        {
                            if (fixtures[j] == excludeFix)
                            {
                                fixtures.RemoveAt(j);
                            }
                        }
                    }
                }
                if (fixtures.Count > 0)
                {
                    return true;
                }

            }
            return false;
        }

        public static string FormatIP(string ip)
        {
            Match match = Regex.Match(ip, @"([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})");
            if (match.Success)
                return ip;
            else return "127.0.0.1";
        }

        public static float getVolume(Vector2 pos, Vector2 centre)
        {
            int near = 200;
            int far = 1500;
            pos = ConvertUnits.ToDisplayUnits(pos);
            centre = ConvertUnits.ToDisplayUnits(centre);
            float distance = Math.Abs((centre - pos).Length());
            if (distance < near)
                return 1;
            else if (distance > far)
                return 0;
            else
            {
                return (far - (distance)) / far;
            }
        }

        public static float getPan(Vector2 pos, Vector2 centre)
        {
            float val = (ConvertUnits.ToDisplayUnits(pos).X - ConvertUnits.ToDisplayUnits(centre).X) / 1000;
            if (val < -1)
                val = -1;
            else if (val > 1)
                val = 1;
            return val;
        }
    }
}
