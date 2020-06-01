using CodeChatSDK.Models;
using Microsoft.EntityFrameworkCore;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Text;

namespace CodeChatSDK.Repository.Sqlite
{
    /// <summary>
    /// 用户数据库上下文
    /// </summary>
    public class AccountContext:DbContext
    {
        /// <summary>
        /// 数据库存储路径
        /// </summary>
        private string path;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">数据库存储路径</param>
        public AccountContext(string path) : base()
        {
            this.path = path;
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(path);
        }

        /// <summary>
        /// 话题表
        /// </summary>
        public DbSet<Topic> Topics { get; set; }

        /// <summary>
        /// 订阅者表
        /// </summary>
        public DbSet<Subscriber> Subscribers { get; set; }

        /// <summary>
        /// 消息表
        /// </summary>
        public DbSet<ChatMessage> Messages { get; set; }

        /// <summary>
        /// 获取用户数据库上下文
        /// </summary>
        /// <returns></returns>
        public AccountContext GetContext()
        {
            return new AccountContext(path);
        }
    }
}
