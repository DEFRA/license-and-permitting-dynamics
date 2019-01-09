// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;

namespace WastePermits.UIAutomation
{
    public static class TestSettings
    {
        public static BrowserOptions Options = new BrowserOptions
        {
            BrowserType = BrowserType.Chrome,
            PrivateMode = true,
            FireEvents = true,
            Headless = false,
            UserAgent = false
        };
    }
}
