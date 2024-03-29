﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Data.Models;
using Data.ViewModel.Task;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class FollowService 
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FollowService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

    }
}
