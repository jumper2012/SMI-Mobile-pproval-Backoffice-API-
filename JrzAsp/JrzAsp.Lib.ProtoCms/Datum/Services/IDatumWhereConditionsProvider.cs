﻿using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Datum.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public interface IDatumWhereConditionsProvider : IGlobalSingletonDependency {
        decimal Priority { get; }
        IEnumerable<DatumWhereCondition> DefineWhereConditions();
    }
}