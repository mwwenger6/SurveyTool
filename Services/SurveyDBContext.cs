using Microsoft.EntityFrameworkCore;
using SurveyTool.Models;

namespace SurveyTool.Services
{
    public class SurveyDbContext(DbContextOptions<SurveyDbContext> options) : DbContext(options)
    {
        public DbSet<TblSurvey> TblSurveys { get; set; }
        public DbSet<TblQuestion> TblQuestions { get; set; }
        public DbSet<TblAnswer> TblAnswers { get; set; }
        public DbSet<TblResponse> TblResponses { get; set; }
        public DbSet<TblResponseAnswer> TblResponseAnswers { get; set; }
    }
}
