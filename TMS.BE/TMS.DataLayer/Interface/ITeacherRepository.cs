﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.DataLayer.Entity;

namespace TMS.DataLayer.Interface
{
    public interface ITeacherRepository : IBaseRepository<Teacher>
    {
        Task<List<Teacher>> GetTeacherByListTeacherCode(List<string> listCode);
    }
}
