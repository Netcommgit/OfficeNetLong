using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using System.Reflection.Emit;

namespace OfficeNet.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<
            ApplicationUser,
            ApplicationRole,
            long,
            IdentityUserClaim<long>,
            IdentityUserRole<long>,
            IdentityUserLogin<long>,
            IdentityRoleClaim<long>,
            IdentityUserToken<long>
        >
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SurveyDetails> SurveyDetail { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<UsersDepartment> UsersDepartments { get; set; }
        public DbSet<SurveyAuthenticateUser> SurveyAuthenticateUsers { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyOption> SurveyOptions { get; set; }
        public DbSet<SurveyList> SurveyListData { get; set; }
        public DbSet<SmtpDetail> SmtpDetails { get; set; }
        public DbSet<SurveyQuestionResponse> SurveyQuestionResponses { get; set; }
        public DbSet<GetSurveyUserList> GetSurveyUserLists { get; set; }
        public DbSet<UsersSurveyList> UsersSurveyLists { get; set; }
        public DbSet<SurveyFlatResult> SurveyFlatResults { get; set; }
        public DbSet<SurveyResult> SurveyResults { get; set; }
        public DbSet<resultCount> ResultCounts { get; set; }
        public DbSet<OpinionPollTopic> OpinionPollTopics { get; set; }
        public DbSet<OpinionPollOption> OpinionPollOptions { get; set; }
        public DbSet<OpinionPollLog> OpinionPollLogs { get; set; }
        public DbSet<OpinionPollAnswer> OpinionPollAnswers { get; set; }
        public DbSet<OpinionResult> OpinionResults { get; set; }
        public DbSet<ActivatePollDTO> ActivatePollDTOs { get; set; }
        public DbSet<EmployeeWiseOpinionResult> EmployeeWiseOpinionResults { get; set; }
        public DbSet<DiscussionTopic> DiscussionTopics { get; set; }
        public DbSet<DiscussionGroup> DiscussionGroups { get; set; }
        public DbSet<DiscussionUser> DiscussionUsers { get; set; }
        public DbSet<DiscussionMessage> DiscussionMessages { get; set; }
        public DbSet<ThoughtOfDay> ThoughtsOfTheDay { get; set; }
        public DbSet<HelpDeskDetailModel> HelpDeskDetails { get; set; }
        public DbSet<HelpdeskAdminUser> HelpdeskAdminUser { get; set; }
        public DbSet<HelpdeskDepartmentModel> HelpdeskDepartments { get; set; }
        public DbSet<HelpdeskCategoryModel> HelpdeskCategories { get; set; }
        public DbSet<HelpdeskSubcategoryModel> HelpdeskSubcategories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<long>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<long>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<long>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<long>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<long>>().ToTable("UserTokens");
            builder.Entity<SurveyList>().HasNoKey().ToView(null);
            builder.Entity<GetSurveyUserList>().HasNoKey().ToView(null);
            builder.Entity<UsersSurveyList>().HasNoKey().ToView(null);
            builder.Entity<SurveyFlatResult>().HasNoKey().ToView(null);
            builder.Entity<SurveyResult>().HasNoKey().ToView(null);
            builder.Entity<resultCount>().HasNoKey().ToView(null);
            builder.Entity<OpinionResult>().HasNoKey().ToView(null);
            builder.Entity<ActivatePollDTO>().HasNoKey().ToView(null);
            builder.Entity<EmployeeWiseOpinionResult>().HasNoKey().ToView(null);

            builder.Entity<OpinionPollTopic>()
                .HasMany(t => t.OpinionPollOptions)
                .WithOne(o => o.OpinionPollTopic)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
