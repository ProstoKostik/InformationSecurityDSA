using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Numerics;

namespace DSA
{
    public partial class FormMain : Form
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
        /// Заданный пользователем параметр q
        /// </summary>
        private BigInteger _tmpQ;

        /// <summary>
        /// Заданный пользователем параметр p
        /// </summary>
        private BigInteger _tmpP;

        /// <summary>
        /// Цифровая подпись
        /// </summary>
        private DigitalSignatureAlgorithm _DSA;

        /// <summary>
        /// Инициализация компонентов формы
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            _tmpP = BigInteger.Zero;
            _tmpQ = BigInteger.Zero;
            _paramQ = new BigInteger(142433);
            _paramT = new BigInteger(6);
            _paramP = _paramQ * _paramT + BigInteger.One;
        }

        /// <summary>
        /// Выбор пользовательских настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserInput_CheckedChanged(object sender, EventArgs e)
        {
            userParams.Enabled = userInput.Checked;
        }

        /// <summary>
        /// Изменение текста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Text_TextChanged(object sender, EventArgs e)
        {
            Generate.Enabled = text.Text.Length != 0;
            if (Generate.Enabled)
            {
                Check.Enabled = gen.Text.Length != 0;
            }
        }

        /// <summary>
        /// Создание электронной подписи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Generate_Click(object sender, EventArgs e)
        {
            if (_paramP <= _paramQ)
            {
                message.Text = "Неверно заданы параметры";
                message.ForeColor = Color.DeepPink;
                message.Visible = true;
            }
            else
            {
                P.Text = _paramP.ToString();
                Q.Text = _paramQ.ToString();
                _DSA = new DigitalSignatureAlgorithm(_paramQ, _paramP);
                StringBuilder sb = _DSA.GenerateDigitalSignature(text.Text);
                gen.Clear();
                gen.AppendText(sb.ToString());
                Check.Enabled = true;
            }
        }

        /// <summary>
        /// Изменение параметра q
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Q_TextChanged(object sender, EventArgs e)
        {
            message.Visible = false;
            if (Q.Text.Length != 0)
            {
                wrongQ.Visible = !BigInteger.TryParse(Q.Text, out _tmpQ);
                if (!wrongQ.Visible)
                {
                    _paramQ = _tmpQ;
                }
            }
            else
            {
                _paramQ = int.MaxValue;
                wrongQ.Visible = false;
            }
        }

        /// <summary>
        /// Изменение параметра p
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void P_TextChanged(object sender, EventArgs e)
        {
            message.Visible = false;
            if (P.Text.Length != 0)
            {
                wrongP.Visible = !BigInteger.TryParse(P.Text, out _tmpP);
                if (wrongP.Visible)
                {
                    _paramP = _tmpP;
                }
            }
            else
            {
                _paramP = _paramQ * _paramT + BigInteger.One;
                wrongP.Visible = false;
            }
        }

        /// <summary>
        /// Проверка электронной подписи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Check_Click(object sender, EventArgs e)
        {
            string sb = _DSA.CheckDigitalSignature(text.Text, gen.Lines);
            if (_DSA.Check)
            {
                message.Text = "Текст корректен";
                message.ForeColor = Color.Blue;
            }
            else
            {
                message.Text = "Текст был изменен или неверно заданы параметры";
                message.ForeColor = Color.Red;
            }
            message.Visible = true;
            ch.Clear();
            ch.AppendText(sb);
        }
    }
}