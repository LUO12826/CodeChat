using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Core.Entities
{
    /// <summary>
    /// 文档编辑类
    /// </summary>
    public class TextDoc
    {
        /// <summary>
        /// 行链表
        /// </summary>
        public LinkedList<TextLine> TextLines { get; set; }

        /// <summary>
        /// 无参构造方法
        /// </summary>
        public TextDoc()
        {
            TextLines = new LinkedList<TextLine>();
            TextLine headLine = new TextLine();
            headLine.Mark = Global.LineMarkType.Head;
            TextLines.AddFirst(headLine);
        }

        /// <summary>
        /// 以一段初始文本段初始化
        /// </summary>
        /// <param name="rawText"></param>
        public TextDoc(string rawText):this()
        {
            int lastEnterIndex = -1;
            int index = 0;
            char[] rawTextArray = rawText.ToCharArray();
            while (index < rawText.Length)
            {
                if (rawText.ElementAt(index)=='\n')
                {
                    TextLine newTextLine = new TextLine(rawText.Substring(lastEnterIndex + 1, index - lastEnterIndex - 1));
                    TextLines.AddLast(newTextLine);
                    lastEnterIndex = index;
                }
                index++;
            }
            TextLine lastTextLine = new TextLine(rawText.Substring(lastEnterIndex + 1, index - lastEnterIndex - 1));
            TextLines.AddLast(lastTextLine);
        }

        /// <summary>
        /// 根据行号(首行为1)，获取对应的抽象行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        public TextLine GetTextLineByIndex(int lineIndex)
        {
            try
            {
                return this.TextLines.ElementAt(lineIndex);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据hash值找到对应行的位置
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public int GetIndexByHash(int hash)
        {
            int i = 0;
            foreach(TextLine line in TextLines)
            {
                if (line.ID == hash)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        /// <summary>
        /// 根据hash获得具体行
        /// </summary>
        /// <param name="hash"></param>
        /// <returns>具体行，如果不存在则返回空</returns>
        public TextLine GetLineByHash(int hash)
        {
            foreach(TextLine line in TextLines)
            {
                if (line.ID == hash)
                    return line;
            }
            return null;
        }

        /// <summary>
        /// 获取能够显示的行的行数
        /// </summary>
        /// <returns></returns>
        public int GetLineCounts()
        {
            int count = this.TextLines.Count()-1;
            foreach(TextLine line in TextLines)
            {
                if (line.Mark == Global.LineMarkType.Deleted || line.Mark == Global.LineMarkType.Unchangeable)
                    count--;
            }
            return count;
        }
        
        /// <summary>
        /// 将行组织为文本，会忽略掉将被删除的行
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder rawText = new StringBuilder();
            foreach(TextLine textLine in TextLines)
            {
                if(textLine.Mark!=Global.LineMarkType.Deleted&&textLine.Mark!=Global.LineMarkType.Head)
                    rawText.Append(textLine.ToString());
            }
            return rawText.ToString();
        }

        /// <summary>
        ///第一个有效行之前的空行
        /// </summary>
        public TextLine HeadLine
        {
            get => TextLines.First();
        }

        /// <summary>
        /// 在行的后面插入新行，并将该新行传递给参数returnLine
        /// </summary>
        /// <param name="targetLine"></param>
        /// <param name="returnLine"></param>
        /// <returns></returns>
        public Boolean InsertLine(TextLine targetLine, out TextLine returnLine)
        {
            if (!TextLines.Contains(targetLine))
            {
                returnLine = null;
                return false;
            }
            returnLine = new TextLine();
            TextLines.AddAfter(TextLines.Find(targetLine), returnLine);
            targetLine.AddNewLine(returnLine);
            return true;
        }

        /// <summary>
        /// 标记一个行，表示要求删除该行
        /// </summary>
        /// <param name="targetLine"></param>
        /// <returns></returns>
        public Boolean MarkDeleteLine(TextLine targetLine)
        {
            if (!TextLines.Contains(targetLine))
            {
                return false;
            }
            targetLine.Mark = Global.LineMarkType.Deleted;
            return true;
        }

        /// <summary>
        /// 收到确认消息后，移除对应行
        /// </summary>
        /// <param name="targetLine"></param>
        /// <returns></returns>
        public Boolean ConfirmDeleteLine(TextLine targetLine)
        {
            if(TextLines.Contains(targetLine)&&targetLine.Mark==Global.LineMarkType.Deleted)
            {
                TextLines.Remove(targetLine);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 直接删除某行
        /// </summary>
        /// <param name="targetLine"></param>
        /// <returns></returns>
        public Boolean DeleteLine(TextLine targetLine)
        {
            if (!TextLines.Contains(targetLine))
                return false;
            TextLines.Remove(targetLine);
            return true;
        }

        /// <summary>
        /// 告知编辑器在某行后添加行，不将该行加入新增行记录队列
        /// </summary>
        /// <param name="line"></param>
        /// <param name="targetLine"></param>
        /// <returns>返回真正执行添加操作的行</returns>
        public TextLine ToAddNewLineAfterLine(TextLine newLine, TextLine oldLine)
        {
            //在目标行后添加新行时需要注意目标行自身也维护有新增行队列，需确认到整个新增递归队列的最后一个元素，对其执行操作
            while (oldLine.NewLines.Count > 0)
            {
                oldLine = oldLine.NewLines.FirstOrDefault();
            }
            TextLines.AddAfter(TextLines.Find(oldLine), newLine);
            return oldLine;
        }

        /// <summary>
        /// 告知编辑器在某行前添加行，不将该行加入新增行记录队列
        /// </summary>
        /// <param name="newLine"></param>
        /// <param name="oldLine"></param>
        public void ToAddNewLineBeforeLine(TextLine newLine, TextLine oldLine)
        {
            TextLines.AddBefore(TextLines.Find(oldLine), newLine);
        }
    }
}
