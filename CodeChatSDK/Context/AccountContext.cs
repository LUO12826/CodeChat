using Microsoft.EntityFrameworkCore;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Text;

namespace CodeChatSDK
{
    /// <summary>
    /// 用户数据库上下文
    /// </summary>
    public class AccountContext:DbContext
    {
        /// <summary>
        /// 数据库存储路径
        /// </summary>
        public static string Path { get; set; }

        /// <summary>
        /// 实例
        /// </summary>
        public static AccountContext Instance
        {
            get
            {
                return new AccountContext();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AccountContext() : base()
        {
            this.Database.EnsureCreated();
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Path);
        }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<ChatMessage> Messages { get; set; }
    }
}
