﻿#region license

// http://www.gnu.org/licenses/gpl-3.0.html GPL v3 or later

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piwik.Analytics.NetCore.Date;
using Piwik.Analytics.NetCore.Parameters;

/// <summary>
/// Piwik - Open source web analytics
/// For more information, see http://piwik.org
/// </summary>

namespace Piwik.Analytics.NetCore.Modules
{
    /// <summary>
    ///     Service Gateway for Piwik ScheduledReports Module API
    ///     For more information, see http://piwik.org/docs/analytics-api/reference
    /// </summary>
    /// <remarks>
    ///     This Analytics API is tested against Piwik 2.13.1
    ///     Implementation missing for ScheduledReports.getReports, ScheduledReports.generateReport and
    ///     ScheduledReports.sendReport
    /// </remarks>
    public class ScheduledReports : PiwikAnalytics
    {
        /// <summary>
        ///     The format of the scheduled report
        /// </summary>
        public enum ReportFormat
        {
            html,
            pdf,
            csv
        }

        /// <summary>
        ///     The type of the scheduled report
        /// </summary>
        public enum ReportType
        {
            email,
            mobile
        }

        /// <summary>
        ///     Enum of all available report statistics
        /// </summary>
        public enum Statistic
        {
            Actions_get,
            Actions_getDownloads,
            Actions_getEntryPageTitles,
            Actions_getEntryPageUrls,
            Actions_getExitPageTitles,
            Actions_getExitPageUrls,
            Actions_getOutlinks,
            Actions_getPageTitles,
            Actions_getPageTitlesFollowingSiteSearch,
            Actions_getPageUrls,
            Actions_getPageUrlsFollowingSiteSearch,
            Actions_getSiteSearchCategories,
            Actions_getSiteSearchKeywords,
            Actions_getSiteSearchNoResultKeywords,
            Contents_getContentNames,
            Contents_getContentPieces,
            CustomVariables_getCustomVariables,
            DevicePlugins_getPlugin,
            DevicesDetection_getBrand,
            DevicesDetection_getBrowserEngines,
            DevicesDetection_getBrowserVersions,
            DevicesDetection_getBrowsers,
            DevicesDetection_getModel,
            DevicesDetection_getOsFamilies,
            DevicesDetection_getOsVersions,
            DevicesDetection_getType,
            Events_getAction,
            Events_getCategory,
            Events_getName,
            Goals_get,
            Goals_getDaysToConversion,
            Goals_getVisitsUntilConversion,
            MultiSites_getAll,
            Provider_getProvider,
            Referrers_getAll,
            Referrers_getCampaigns,
            Referrers_getKeywords,
            Referrers_getReferrerType,
            Referrers_getSearchEngines,
            Referrers_getSocials,
            Referrers_getWebsites,
            Resolution_getConfiguration,
            Resolution_getResolution,
            UserCountry_getCity,
            UserCountry_getContinent,
            UserCountry_getCountry,
            UserCountry_getRegion,
            UserLanguage_getLanguage,
            UserLanguage_getLanguageCode,
            VisitFrequency_get,
            VisitTime_getByDayOfWeek,
            VisitTime_getVisitInformationPerLocalTime,
            VisitTime_getVisitInformationPerServerTime,
            VisitorInterest_getNumberOfVisitsByDaysSinceLast,
            VisitorInterest_getNumberOfVisitsByVisitCount,
            VisitorInterest_getNumberOfVisitsPerPage,
            VisitorInterest_getNumberOfVisitsPerVisitDuration,
            VisitsSummary_get
        }

        private const string PLUGIN = "ScheduledReports";

        protected override string GetPlugin()
        {
            return PLUGIN;
        }

        /// <summary>
        ///     Add a new scheduled report
        /// </summary>
        /// <param name="idSite">ID of the piwik site</param>
        /// <param name="description">Description of the report</param>
        /// <param name="period">A piwik period</param>
        /// <param name="hour">Defines the hour at which the report will be sent</param>
        /// <param name="reportType">The report type</param>
        /// <param name="reportFormat">The report format</param>
        /// <param name="includedStatistics">The included statistics</param>
        /// <param name="emailMe">true if the report should be sent to the own user</param>
        /// <param name="additionalEmails">A string array of additional email recipients</param>
        /// <returns>The ID of the added report</returns>
        public async Task<int> AddReportAsync(
            int idSite,
            string description,
            PiwikPeriod period,
            int hour,
            ReportType reportType,
            ReportFormat reportFormat,
            List<Statistic> includedStatistics,
            bool emailMe,
            string[] additionalEmails = null
            )
        {
            var additionalParameters = new Dictionary<string, object>
            {
                {"emailMe", emailMe.ToString().ToLower()},
                {"displayFormat", 1},
                {"additionalEmails", additionalEmails}
            };

            Parameter[] p =
            {
                new SimpleParameter("idSite", idSite),
                new SimpleParameter("description", description),
                new PeriodParameter(period),
                new SimpleParameter("hour", hour),
                new SimpleParameter("reportType", reportType.ToString()),
                new SimpleParameter("reportFormat", reportFormat.ToString()),
                new ArrayParameter("reports", includedStatistics.Select(i => i.ToString()).ToArray(), false),
                new DictionaryParameter("parameters", additionalParameters)
            };

            return await SendRequestAsync<int>("addReport", p);
        }

        /// <summary>
        ///     Update an existing scheduled report
        /// </summary>
        /// <param name="idReport">The ID of the report to update</param>
        /// <param name="idSite">ID of the piwik site</param>
        /// <param name="description">Description of the report</param>
        /// <param name="period">A piwik period</param>
        /// <param name="hour">Defines the hour at which the report will be sent</param>
        /// <param name="reportType">The report type</param>
        /// <param name="reportFormat">The report format</param>
        /// <param name="includedStatistics">The included statistics</param>
        /// <param name="emailMe">true if the report should be sent to the own user</param>
        /// <param name="additionalEmails">A string array of additional email recipients</param>
        /// <returns>True if update was successful</returns>
        public async Task<bool> UpdateReportAsync(
            int idReport,
            int idSite,
            string description,
            PiwikPeriod period,
            int hour,
            ReportType reportType,
            ReportFormat reportFormat,
            List<Statistic> includedStatistics,
            bool emailMe,
            string[] additionalEmails = null
            )
        {
            var additionalParameters = new Dictionary<string, object>
            {
                {"emailMe", emailMe.ToString().ToLower()},
                {"displayFormat", 1},
                {"additionalEmails", additionalEmails}
            };

            Parameter[] p =
            {
                new SimpleParameter("idReport", idReport),
                new SimpleParameter("idSite", idSite),
                new SimpleParameter("description", description),
                new PeriodParameter(period),
                new SimpleParameter("hour", hour),
                new SimpleParameter("reportType", reportType.ToString()),
                new SimpleParameter("reportFormat", reportFormat.ToString()),
                new ArrayParameter("reports", includedStatistics.Select(i => i.ToString()).ToArray(), false),
                new DictionaryParameter("parameters", additionalParameters)
            };

            return await SendRequestAsync<bool>("updateReport", p);
        }

        /// <summary>
        ///     Remove a scheduled report
        /// </summary>
        /// <param name="idReport">The ID of the report to delete</param>
        /// <returns>True if the deletion was successful</returns>
        public async Task<bool> DeleteReportAsync(int idReport)
        {
            Parameter[] parameters =
            {
                new SimpleParameter("idReport", idReport)
            };

            return await SendRequestAsync<bool>("deleteReport", parameters);
        }
    }
}