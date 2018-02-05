﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dichotomie.Models;
using DichotomieWeb.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using DichotomieWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace DichotomieWeb.Pages.Forum.Replies
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int TopicId { get; set; }
        public IList<Reply> Replies { get;set; }
        public IList<SelectListItem> ReputationList { get; set; }

        public async Task OnGetAsync(int topicId, int votevalue, int replyid)
        {
            ReputationList = new List<SelectListItem>();

            TopicId = topicId;
            Replies = await _context.Replies
                .Include(r => r.Topic)
                .Include(r => r.User)
                .Where(r => r.Topic.TopicId == topicId)
                .OrderBy(r => r.CreationDate)
                .ToListAsync();

            ReputationList.Add(new SelectListItem { Value = "", Text = "" });
            for (int i = 0; i <= 5; i++)
            {
                ReputationList.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }

            if (votevalue != 0 && replyid != 0)         
                {
                    ApplicationUser usertmp = new ApplicationUser();
                    foreach (Reply reply in Replies)
                    {
                        if (reply.ReplieId == replyid)
                        {
                            usertmp = reply.User; 
                        }
                    }
                    Reputation rep = new Reputation
                    {
                        User = usertmp,
                        FromUser = await _userManager.GetUserAsync(User),
                        MarkValue = votevalue,
                    };
                    _context.Reputations.Add(rep);
                    _context.SaveChanges();
            }
        }
    }
}
