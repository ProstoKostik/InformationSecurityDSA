using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Numerics;
using System.Windows.Forms;

namespace DSA
{
    /// <summary>
    /// Класс, реализующий DSA
    /// </summary>
    public class DigitalSignatureAlgorithm
    {
        /// <summary>
        /// Параметр q
        /// </summary>
        private BigInteger _paramQ;
        /// <summary>
        /// Параметр p
        /// </summary>
        private BigInteger _paramP;
        /// <summary>
        /// Результат частного (p-1)/q
        /// </summary>
        private BigInteger _paramT;
        /// <summary>
        /// Параметр g
        /// </summary>
        private BigInteger _paramG;
        /// <summary>
        /// Закрытый ключ
        /// </summary>
        private BigInteger _paramX;
        /// <summary>
        /// Параметр y
        /// </summary>
        private BigInteger _paramY;

        /// <summary>
        /// DSA с параметрами q и p
        /// </summary>
        /// <param name="q">q</param>
        /// <param name="p">p</param>
        public DigitalSignatureAlgorithm(BigInteger q, BigInteger p)
        {
            _paramQ = q;
            _paramP = p;
            _paramT = (p - BigInteger.One) / q;
            SetParamG();
            _paramX = GetRandom(_paramQ);
            _paramY = BigInteger.ModPow(_paramG, _paramX, _paramP);
        }

        /// <summary>
        /// Устанавливает рандомное значение заданного параметра, которое больше 0 и не превышает maxValue
        /// </summary>
        /// <param name="maxValue">Верхняя граница параметра</param>
        private BigInteger GetRandom(BigInteger maxValue)
        {
            Random rand = new Random();
            double tmp1 = rand.NextDouble();
            double tmp2 = rand.NextDouble();
            int tmp3 = rand.Next();

            BigInteger b1 = new BigInteger(UInt64.MaxValue * tmp1);
            BigInteger b2 = new BigInteger(UInt64.MaxValue * tmp2);
            BigInteger b3 = BigInteger.ModPow(10, tmp3, _paramQ);
            BigInteger result = (b1 * b3 + b2) % (_paramQ - 1);
            if (result == BigInteger.Zero) {
                return BigInteger.One;
            }
            return result;
        }
        /// <summary>
        /// Задать число g, такого, что  его мультипликативный порядок по модулю p равен q
        /// </summary>
        private void SetParamG()
        {
            BigInteger h = BigInteger.One;
            do
            {
                h += BigInteger.One;
                _paramG = BigInteger.ModPow(h, _paramT, _paramP);
            }
            while (_paramG < 2);
        }
        /// <summary>
        /// Подпись сообщения
        /// </summary>
        /// <param name="text">Текст для подписи</param>
        /// <returns>Подпись, пара r и s</returns>
        public StringBuilder GenerateDigitalSignature(string text)
        {

            BigInteger H = GetHashCode(text);
            BigInteger k;
            BigInteger r;
            BigInteger s;
            do
            {
                do
                {
                    //Выбор случайного числа, от 0 до q
                    k = GetRandom(_paramQ);
                    //Вычисление параметра r
                    r = BigInteger.ModPow(_paramG, k, _paramP) % _paramQ;
                }
                while (r == BigInteger.Zero);
                //Вычисление параметра s
                s = (BigInteger.ModPow(k, _paramQ - 2, _paramQ) * ((H + _paramX * r) % _paramQ)) % _paramQ;
            }
            while (s == BigInteger.Zero);

            StringBuilder SB = new StringBuilder();
            SB.AppendLine("r = " + r.ToString());
            SB.AppendLine("s = " + s.ToString());
            //Подпись
            return SB;
        }
        /// <summary>
        /// Проверка подписи
        /// </summary>
        /// <param name="text">Текст для проверки </param>
        /// <param name="key">Ключ</param>
        /// <param name="message">Сообщение</param>
        /// <returns>Результат проверки по внешнему ключу</returns>
        public StringBuilder CheckDigitalSignature(string text, string[] key, ref Label message)
        {
            BigInteger r = BigInteger.Parse(key[0].Substring(4));
            BigInteger s = BigInteger.Parse(key[1].Substring(4));
            BigInteger H = GetHashCode(text);
           
            //Вычисление параметра w
            BigInteger w = BigInteger.ModPow(s, _paramQ - 2, _paramQ);
            //Вычисление параметра u1
            BigInteger u1 = (H * w) % _paramQ;
            //Вычисление параметра u2
            BigInteger u2 = (r * w) % _paramQ;
            //Вычисление параметра v
            BigInteger v = GetParamV(u1, u2);
            //Если параметр v = r, то подпись верна
            if (v == r)
            {
                message.Text = "Текст корректен";
                message.ForeColor = System.Drawing.Color.Blue;
                message.Visible = true;
            }
            else
            {
                message.Text = "Текст был изменен или неверно заданы параметры";
                message.ForeColor = System.Drawing.Color.Red;
                message.Visible = true;
            }
            return new StringBuilder("v = " + v.ToString());
        }
        /// <summary>
        /// Получить параметр v
        /// </summary>
        /// <param name="u1">Параметр u1</param>
        /// <param name="u2">Параметр u2</param>
        /// <returns>Параметр v</returns>
        private BigInteger GetParamV(BigInteger u1, BigInteger u2)
        {
            BigInteger v1 = BigInteger.ModPow(_paramG, u1, _paramP);
            BigInteger v2 = BigInteger.ModPow(_paramY, u2, _paramP);
            BigInteger v3 = (v1 * v2) % _paramP;

            return v3 % _paramQ; 
        }

        /// <summary>
        /// Получить хеш-код сообщения
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns>Хеш-код</returns>
        private BigInteger GetHashCode(string text)
        {
            SHA256 hash = new SHA256Managed();
            byte[] hashBYTE = hash.ComputeHash(Encoding.Default.GetBytes(text));
            BigInteger hashINT = new BigInteger(hashBYTE);

            BigInteger result = hashINT & _paramQ;
            return result;
        }

    }
}
 