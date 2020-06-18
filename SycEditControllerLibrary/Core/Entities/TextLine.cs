using SynEditControllerLibrary.Core.Global;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Services.Maps;
using Windows.UI.Xaml;

namespace SynEditControllerLibrary.Core.Entities
{
    /// <summary>
    /// 行编辑类
    /// </summary>
    public class TextLine
    {
        /// <summary>
        /// 每行的独立标识
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 脏位
        /// </summary>
        public LineMarkType Mark { get; set; }

        /// <summary>
        /// 新增行的标记队列
        /// </summary>
        public Queue<TextLine> NewLines { get; }

        /// <summary>
        /// 行的内容
        /// </summary>
        private string content;

        /// <summary>
        /// 行内最大容量
        /// </summary>
        private static int MAX_LENGTH;

        /// <summary>
        /// 行的当前文本长度
        /// </summary>
        public int Length
        {
            get => content.Length;
        }

        /// <summary>
        /// 获取行内容
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            return content;
        }

        /// <summary>
        /// 静态构造方法
        /// </summary>
        static TextLine()
        {
            try
            {
                GetConfig();
            }
            catch (Exception)
            {
                Debug.WriteLine("配置TextLine时出错");
                //TODO
            }
        }

        /// <summary>
        /// 无参构造方法
        /// </summary>
        public TextLine()
        {
            content = "";
            ID = HashCode.Combine(base.GetHashCode(),DateTime.Now);
            NewLines = new Queue<TextLine>();
            Mark = LineMarkType.UnChanged;
        }

        /// <summary>
        /// 带初始化文本的构造方法
        /// </summary>
        /// <param name="rawLine">原始文本</param>
        public TextLine(string rawLine):this()
        {
            content = rawLine;
        }

        /// <summary>
        /// 复制一个新行
        /// </summary>
        /// <param name="ID"></param>
        public TextLine(int ID)
        {
            content = "";
            this.ID = ID;
            NewLines = new Queue<TextLine>();
            Mark = LineMarkType.UnChanged;
        }

        /// <summary>
        /// 复制一个带初始值的新行
        /// </summary>
        /// <param name="rawLine"></param>
        /// <param name="ID"></param>
        public TextLine(string rawLine, int ID):this(ID)
        {
            content = rawLine;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        private static void GetConfig()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "config.xml";
            XDocument xmlDoc = XDocument.Load(path);
            try
            {
                MAX_LENGTH = Int32.Parse(xmlDoc.Element("Line").Attribute("MaxLength").ToString());
            }catch(Exception)
            {
                MAX_LENGTH = 0;
                Debug.WriteLine("参数错误: 配置文件中的MaxLength值只能为Int16类型");
            }
        }

        #region Operation of Content

        /// <summary>
        /// 改变行文本的全部内容
        /// </summary>
        /// <param name="newContent">新增内容</param>
        public void EditContent(string newContent)
        {
            content = newContent;
        }

        /// <summary>
        /// 行尾添加一个字符
        /// </summary>
        /// <param name="newWord">新增词</param>
        /// <returns>如果成功则返回真</returns>
        public Boolean AppendContent(char newWord)
        {
            if (MAX_LENGTH > 0 && this.Length >= MAX_LENGTH)
                return false;
            content += newWord;
            return true;
        }

        /// <summary>
        /// 添加多个字符
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="newWord">新增词</param>
        /// <returns>添加成功则返回真</returns>
        public Boolean AppendContent(int index, string newWord)
        {
            if (MAX_LENGTH > 0 && this.Length + newWord.Length > MAX_LENGTH)
                return false;
            if (index < 0 || index > this.Length)
                return false;
            content.Insert(index, newWord);
            return true;
        }

        /// <summary>
        /// 删除行尾的一个字符
        /// </summary>
        /// <returns>删除成功则返回真</returns>
        public Boolean BackspaceContent()
        {
            if (this.Length <= 0)
                return false;
            content.Remove(content.Length - 1);
            return true;
        }

        /// <summary>
        /// 删除多个字符
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="count">字符个数</param>
        /// <returns>删除成功则返回真</returns>
        public Boolean BackspaceContent(int index, int count)
        {
            if (this.Length - count < 0)
                return false;
            if (index < 0 || index > this.Length - count)
                return false;
            content.Remove(index, count);
            return true;
        }

        /// <summary>
        /// 行后添加新行时调用此方法标记
        /// </summary>
        /// <param name="newLine">新增的行</param>
        /// <returns>添加成功则返回真</returns>
        public Boolean AddNewLine(TextLine newLine)
        {
            if (NewLines.Contains(newLine))
                return false;
            NewLines.Enqueue(newLine);
            return true;
        }

        /// <summary>
        /// 收到确认信息或增添新行消息时调用此方法删除标记
        /// </summary>
        /// <param name="newLine">新增行返回值</param>
        /// <returns>确认成功则返回真</returns>
        public Boolean ConfirmNewLine(out TextLine newLine)
        {
            if (NewLines.Count <= 0)
            {
                newLine = null;
                return false;
            }
            newLine = NewLines.Dequeue();
            return true;
        }

        #endregion

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="obj">对比目标</param>
        /// <returns>相等则返回真</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetHashCode() == this.GetHashCode())
                return true;
            return false;
        }

        /// <summary>
        /// 获取hash值
        /// </summary>
        /// <returns>目标hash值</returns>
        public override int GetHashCode()
        {
            return this.ID;
        }

        /// <summary>
        /// 获取文本内容
        /// </summary>
        /// <returns>文本内容</returns>
        public override string ToString()
        {
            return this.content;
        }
    }
}
