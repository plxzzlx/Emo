using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EmoDisplay.Utils
{
    /// <summary>
    /// Complex 的摘要说明。
    /// </summary>
    public class CComplex
    {
        //default constructor method
        public CComplex()
            : this(0, 0)
        {
        }
        //construct method with real part only
        public CComplex(double real)
            : this(real, 0)
        {
        }
        //construct method with both real part and image part
        public CComplex(double real, double image)
        {
            this.real = real;
            this.image = image;
        }

        //the Real property for member variable real
        public double Real
        {
            get
            {
                return this.real;
            }
            set
            {
                this.real = value;
            }
        }

        //the Image property for member variable image
        public double Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }

        //seek the sum value of c1 and c2
        public static CComplex Add(CComplex c1, CComplex c2)
        {
            return new CComplex(c1.Real + c2.Real, c1.Image + c2.Image);
        }

        //seek the minus value of c1 and c2
        public static CComplex Minus(CComplex c1, CComplex c2)
        {
            return new CComplex(c1.Real - c2.Real, c1.Image - c2.Image);
        }

        //overload the operator "+"
        public static CComplex operator +(CComplex c1, CComplex c2)
        {
            return CComplex.Add(c1, c2);//new Complex(c1.Real + c2.Real, c1.Image + c2.Image);
        }

        //overload the operator "-"
        public static CComplex operator -(CComplex c1, CComplex c2)
        {
            return CComplex.Minus(c1, c2);//new Complex(c1.Real - c2.Real, c1.Image - c2.Image);
        }

        //Overload the operator "*"
        public static CComplex operator *(CComplex c1, CComplex c2)
        {
            double a = c1.Real;
            double b = c1.Image;
            double c = c2.Real;
            double d = c2.Image;
            return new CComplex(a * c - b * d, a * d + b * c);
        }

        //Overload the operator /
        public static CComplex operator /(CComplex c1, CComplex c2)
        {
            double a = c1.Real;
            double b = c1.Image;
            double c = c2.Real;
            double d = c2.Image;
            return new CComplex(
                (a * c + b * d) / (Math.Pow(c, 2) + Math.Pow(d, 2)),
                (b * c - a * d) / (Math.Pow(c, 2) + Math.Pow(d, 2)));
        }

        //overload the operator "=="
        public static bool operator ==(CComplex c1, CComplex c2)
        {
            if (ReferenceEquals(c1, c2))
            {
                return true;
            }
            if (!ReferenceEquals(c1, null))
            {
                return c1.Equals(c2);
            }
            return false;
        }

        //overload the operator "!="
        public static bool operator !=(CComplex c1, CComplex c2)
        {
            return !(c1.Equals(c2));
        }

        //overload the method Equals,assert if this equals c or not
        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            CComplex c = (CComplex)o;
            return (this.Real == c.Real) && (this.Image == c.Image);
        }

        //overload the method ToString
        public override string ToString()
        {
            return ResultFormat();
        }

        //format the result
        public string ResultFormat()
        {
            if (Real == 0 && Image == 0)
            {
                return string.Format("{0}", 0);
            }
            if (Real == 0 && (Image != 1 && Image != -1))//Math.Abs(Image) != 1)
            {
                return string.Format("{0} i", Image);
            }
            if (Image == 0)
            {
                return string.Format("{0}", Real);
            }
            if (Image == 1)
            {
                return string.Format("i");
            }
            if (Image == -1)
            {
                return string.Format("- i");
            }

            if (Image < 0)
            {
                return string.Format("{0} - {1} i", Real, -Image);
            }

            return string.Format("{0} + {1} i", Real, Image);
        }

        // uncomment the GetHashCode function to resolve
        public override int GetHashCode()
        {
            return 0;
        }

        private double real;
        private double image;
    }

    public class Complex
    {
        double re;
        double im;

        /// <summary>
        /// Contains the real part of a complex number.
        /// </summary>
        public double Re
        {
            get { return re; }
            set { re = value; }
        }

        /// <summary>
        /// Contains the imaginary part of a complex number.
        /// </summary>
        public double Im
        {
            get { return im; }
            set { im = value; }
        }

        /// <summary>
        /// Imaginary unit.
        /// </summary>
        public static Complex I
        {
            get
            {
                return new Complex(0, 1);
            }
        }

        /// <summary>
        /// Complex number zero.
        /// </summary>
        public static Complex Zero
        {
            get
            {
                return new Complex(0, 0);
            }
        }

        /// <summary>
        /// Complex number one.
        /// </summary>
        public static Complex One
        {
            get
            {
                return new Complex(1, 0);
            }
        }

        #region Constructors

        /// <summary>
        /// Inits complex number as (0, 0).
        /// </summary>
        public Complex()
        {
            Re = 0;
            Im = 0;
        }

        /// <summary>
        /// Inits complex number with imaginary part = 0.
        /// </summary>
        /// <param name="real_part"></param>
        public Complex(double real_part)
        {
            Re = real_part;
            Im = 0;
        }

        /// <summary>
        /// Inits complex number.
        /// </summary>
        /// <param name="imaginary_part"></param>
        /// <param name="real_part"></param>
        public Complex(double real_part, double imaginary_part)
        {
            Re = real_part;
            Im = imaginary_part;
        }

        /// <summary>
        /// Inits complex number from string like "a+bi".
        /// </summary>
        /// <param name="s"></param>
        public Complex(string s)
        {
            throw new NotImplementedException();
        }

        public static Match Test(string s)
        {

            string dp = "([0-9]+[.]?[0-9]*|[.][0-9]+)";
            string dm = "[-]?" + dp;

            Regex r = new Regex("^(?<RePart>(" + dm + ")[-+](?<ImPart>(" + dp + "))[i])$");

            return r.Match(s);
        }

        #endregion

        #region Operators

        public static Complex operator +(Complex a, Complex b)
        {
            //if (a == null) return b;
            //else if (b == null) return a;
            //else 
            return new Complex(a.Re + b.Re, a.Im + b.Im);
        }

        public static Complex operator +(Complex a, double b)
        {
            return new Complex(a.Re + b, a.Im);
        }

        public static Complex operator +(double a, Complex b)
        {
            return new Complex(a + b.Re, b.Im);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Re - b.Re, a.Im - b.Im);
        }

        public static Complex operator -(Complex a, double b)
        {
            return new Complex(a.Re - b, a.Im);
        }

        public static Complex operator -(double a, Complex b)
        {
            return new Complex(a - b.Re, -b.Im);
        }

        public static Complex operator -(Complex a)
        {
            return new Complex(-a.Re, -a.Im);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Re * b.Re - a.Im * b.Im,
                a.Im * b.Re + a.Re * b.Im);
        }

        public static Complex operator *(Complex a, double d)
        {
            return new Complex(d) * a;
        }

        public static Complex operator *(double d, Complex a)
        {
            return new Complex(d) * a;
        }

        public static Complex operator /(Complex a, Complex b)
        {
            return a * Conj(b) * (1 / (Abs(b) * Abs(b)));
        }

        public static Complex operator /(Complex a, double b)
        {
            return a * (1 / b);
        }

        public static Complex operator /(double a, Complex b)
        {
            return a * Conj(b) * (1 / (Abs(b) * Abs(b)));
        }

        public static bool operator ==(Complex a, Complex b)
        {
            return (a.Re == b.Re && a.Im == b.Im);
        }

        public static bool operator ==(Complex a, double b)
        {
            return a == new Complex(b);
        }

        public static bool operator ==(double a, Complex b)
        {
            return new Complex(a) == b;
        }

        public static bool operator !=(Complex a, Complex b)
        {
            return !(a == b);
        }

        public static bool operator !=(Complex a, double b)
        {
            return !(a == b);
        }

        public static bool operator !=(double a, Complex b)
        {
            return !(a == b);
        }

        #endregion

        #region Static funcs & overrides

        /// <summary>
        /// Calcs the absolute value of a complex number.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Abs(Complex a)
        {
            return Math.Sqrt(a.Im * a.Im + a.Re * a.Re);
        }

        /// <summary>
        /// Inverts a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Inv(Complex a)
        {
            return new Complex(a.Re / (a.Re * a.Re + a.Im * a.Im),
                -a.Im / (a.Re * a.Re + a.Im * a.Im));
        }

        /// <summary>
        /// Tangent of a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Tan(Complex a)
        {
            return Sin(a) / Cos(a);
        }

        /// <summary>
        /// Hyperbolic cosine of a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Cosh(Complex a)
        {
            return (Exp(a) + Exp(-a)) / 2;
        }

        /// <summary>
        /// Hyperbolic sine of a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Sinh(Complex a)
        {
            return (Exp(a) - Exp(-a)) / 2;
        }

        /// <summary>
        /// Hyperbolic tangent of a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Tanh(Complex a)
        {
            return (Exp(2 * a) - 1) / (Exp(2 * a) + 1);
        }

        /// <summary>
        /// Hyperbolic cotangent of a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Coth(Complex a)
        {
            return (Exp(2 * a) + 1) / (Exp(2 * a) - 1);
        }

        /// <summary>
        /// Hyperbolic secant of a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Sech(Complex a)
        {
            return Inv(Cosh(a));
        }

        /// <summary>
        /// Hyperbolic cosecant of a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Csch(Complex a)
        {
            return Inv(Sinh(a));
        }

        /// <summary>
        /// Cotangent of a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Cot(Complex a)
        {
            return Cos(a) / Sin(a);
        }

        /// <summary>
        /// Computes the conjugation of a complex number.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Conj(Complex a)
        {
            return new Complex(a.Re, -a.Im);
        }

        /// <summary>
        /// Complex square root.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Complex Sqrt(double d)
        {
            if (d >= 0)
                return new Complex(Math.Sqrt(d));
            else
                return new Complex(0, Math.Sqrt(-d));
        }

        /// <summary>
        /// Complex square root.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>   
        public static Complex Sqrt(Complex a)
        {
            return Pow(a, .5);
        }

        /// <summary>
        /// Complex exponential function.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Exp(Complex a)
        {
            return new Complex(Math.Exp(a.Re) * Math.Cos(a.Im), Math.Exp(a.Re) * Math.Sin(a.Im));
        }

        /// <summary>
        /// Main value of the complex logarithm.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Log(Complex a)
        {
            // Log[|w|]+I*(Arg[w]+2*Pi*k)            

            return new Complex(Math.Log(Abs(a)), Arg(a));
        }

        /// <summary>
        /// Argument of the complex number.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Arg(Complex a)
        {
            if (a.Re < 0)
            {
                if (a.Im < 0)
                    return Math.Atan(a.Im / a.Re) - Math.PI;
                else
                    return Math.PI - Math.Atan(-a.Im / a.Re);
            }
            else
                return Math.Atan(a.Im / a.Re);

        }

        /// <summary>
        /// Complex cosine.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Cos(Complex a)
        {
            return .5 * (Exp(Complex.I * a) + Exp(-Complex.I * a));
        }

        /// <summary>
        /// Complex sine.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Sin(Complex a)
        {
            return (Exp(Complex.I * a) - Exp(-Complex.I * a)) / (2 * Complex.I);
        }

        public static Complex Pow(Complex a, Complex b)
        {
            return Exp(b * Log(a));
        }

        public static Complex Pow(double a, Complex b)
        {
            return Exp(b * Math.Log(a));
        }

        public static Complex Pow(Complex a, double b)
        {
            return Exp(b * Log(a));
        }

        public override string ToString()
        {
            if (this == Complex.Zero) return "0";

            string re, im, sign;

            if (this.Im < 0)
            {
                if (this.Re == 0)
                    sign = "-";
                else
                    sign = " - ";
            }
            else if (this.Im > 0 && this.Re != 0) sign = " + ";
            else sign = "";

            if (this.Re == 0) re = "";
            else re = this.Re.ToString();

            if (this.Im == 0) im = "";
            else if (this.Im == -1 || this.Im == 1) im = "i";
            else im = Math.Abs(this.Im).ToString() + "i";

            return re + sign + im;
        }

        public string ToString(string format)
        {
            if (this == Complex.Zero) return "0";
            else if (double.IsInfinity(this.Re) || double.IsInfinity(this.Im)) return "oo";
            else if (double.IsNaN(this.Re) || double.IsNaN(this.Im)) return "?";

            string re, im, sign;

            string imval = Math.Abs(this.Im).ToString(format);
            string reval = this.Re.ToString(format);

            if (imval.StartsWith("-"))
            {
                if (reval == "0") sign = "-";
                else sign = " - ";
            }
            else if (imval != "0" && reval != "0")
            {
                sign = " + ";
            }
            else sign = "";

            if (imval == "0") im = "";
            else if (imval == "1") im = "i";
            else im = imval + "i";

            if (reval == "0")
            {
                if (imval != "0")
                    re = "";
                else
                    re = "0";
            }
            else re = reval;



            return re + sign + im;
        }

        public override bool Equals(object obj)
        {
            return obj.ToString() == this.ToString();
        }

        public override int GetHashCode()
        {
            return -1;
        }

        #endregion

        #region Dynamics

        public bool IsReal()
        {
            return (this.Im == 0);
        }

        public bool IsImaginary()
        {
            return (this.Re == 0);
        }


        #endregion
    }
}
