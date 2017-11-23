using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.QueryableUtilities;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumSortHandler : BaseDatumSortHandler<ApplicationUser> {

        private readonly ApplicationDbContext _dbContext;

        public UserDatumSortHandler(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        public override decimal Priority => 0;

        public override IQueryable<ApplicationUser> HandleSort(IQueryable<ApplicationUser> currentQuery,
            string fieldName,
            bool descending, Type datumType,
            out bool callNextHandler) {
            callNextHandler = true;
            if (fieldName == nameof(ApplicationUser.IsActivated)) {
                return currentQuery.AddOrderBy(x => x.IsActivated, descending);
            }
            if (fieldName == nameof(ApplicationUser.UserName)) {
                return currentQuery.AddOrderBy(x => x.UserName, descending);
            }
            if (fieldName == nameof(ApplicationUser.DisplayName)) {
                return currentQuery.AddOrderBy(x => x.DisplayName, descending);
            }
            if (fieldName == nameof(ApplicationUser.Email)) {
                return currentQuery.AddOrderBy(x => x.Email, descending);
            }
            if (fieldName == nameof(ApplicationUser.EmailConfirmed)) {
                return currentQuery.AddOrderBy(x => x.EmailConfirmed, descending);
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumber)) {
                return currentQuery.AddOrderBy(x => x.PhoneNumber, descending);
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumberConfirmed)) {
                return currentQuery.AddOrderBy(x => x.PhoneNumberConfirmed, descending);
            }
            if (fieldName == nameof(ApplicationUser.CreatedUtc)) {
                return currentQuery.AddOrderBy(x => x.CreatedUtc, descending);
            }
            if (fieldName == nameof(ApplicationUser.UpdatedUtc)) {
                return currentQuery.AddOrderBy(x => x.UpdatedUtc, descending);
            }
            if (fieldName == nameof(ApplicationUser.Id)) {
                return currentQuery.AddOrderBy(x => x.Id, descending);
            }
            return null;
        }
    }
}