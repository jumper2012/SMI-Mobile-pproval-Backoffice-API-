using System;
using System.Linq;

namespace JrzAsp.Lib.QueryableUtilities {
    public abstract class PaginatedQueryable : IPaginatedQueryable {
        protected int _count;
        protected int _currentPage;
        protected bool _isCalculated;
        protected int _limit;
        protected int _offset;
        protected int _startNumbering;
        protected int _totalPage;

        public int CurrentPage {
            get {
                Calculate();
                return _currentPage;
            }
            protected set => _currentPage = value;
        }
        public int TotalPage {
            get {
                Calculate();
                return _totalPage;
            }
            protected set => _totalPage = value;
        }
        public int Offset {
            get {
                Calculate();
                return _offset;
            }
            protected set => _offset = value;
        }
        public int Limit {
            get {
                Calculate();
                return _limit;
            }
            protected set => _limit = value;
        }
        public int Count {
            get {
                Calculate();
                return _count;
            }
            protected set => _count = value;
        }
        public int StartNumbering {
            get {
                Calculate();
                return _startNumbering;
            }
            protected set => _startNumbering = value;
        }
        public virtual IQueryable CurrentPageQueryableBase { get; }

        protected abstract void Calculate();
    }
    public class PaginatedQueryable<TData> : PaginatedQueryable, IPaginatedQueryable<TData> {
        protected readonly object CalculateLock = new object();
        protected readonly int InputCurrentPage;
        protected readonly int InputLimitPerPage;
        protected readonly IQueryable<TData> InputQueryableData;
        private IQueryable<TData> _currentPageQueryable;

        public PaginatedQueryable(IQueryable<TData> queryableData, int currentPage, int limitPerPage) {
            InputQueryableData = queryableData;
            InputCurrentPage = currentPage;
            InputLimitPerPage = limitPerPage;
        }

        public IQueryable<TData> CurrentPageQueryable {
            get {
                Calculate();
                return _currentPageQueryable;
            }
            protected set => _currentPageQueryable = value;
        }

        public override IQueryable CurrentPageQueryableBase => CurrentPageQueryable;

        protected override void Calculate() {
            lock (CalculateLock) {
                if (_isCalculated) return;

                var q = InputQueryableData;

                _limit = InputLimitPerPage >= 0 ? InputLimitPerPage : 1;

                _count = q.Count();

                _totalPage = Convert.ToInt32(Math.Ceiling((double) _count / _limit));

                _currentPage = InputCurrentPage;

                if (InputCurrentPage < 1) {
                    _currentPage = 1;
                }

                if (InputCurrentPage > _totalPage) {
                    _currentPage = _totalPage;
                }

                var offset = (_currentPage - 1) * _limit;
                _offset = offset >= 0 ? offset : 0;

                var isOrdered = q.IsOrdered();
                if (!isOrdered) q = q.OrderBy(x => 0);
                _currentPageQueryable = q.Skip(_offset).Take(_limit);

                _startNumbering = _offset + 1;

                _isCalculated = true;
            }
        }
    }
}