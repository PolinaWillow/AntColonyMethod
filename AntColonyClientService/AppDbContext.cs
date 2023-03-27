using AntColonyClient.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Service
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options){}

        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<TaskParams> TaskParams { get; set; }
        public DbSet<ParamElems> ParamElems { get; set; }
    }
}
