﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace KMSGuildExtractor.Core.Requester
{
    public class BaseRequester
    {
        protected static readonly HtmlWeb s_web = new HtmlWeb();
    }
}
