//
// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       christiank, timop
//
// Copyright 2004-2014 by OM International
//
// This file is part of OpenPetra.org.
//
// OpenPetra.org is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// OpenPetra.org is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with OpenPetra.org.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Ict.Common;
using Ict.Common.Verification;
using Ict.Common.Remoting.Shared;
using Ict.Petra.Client.App.Core.RemoteObjects;
using Ict.Petra.Shared.Interfaces.MSysMan;
using Ict.Petra.Shared.MSysMan.Data;
using Ict.Petra.Shared;

namespace Ict.Petra.Client.App.Core
{
    /// <summary>
    /// Loads and saves all user-customisable defaults.
    /// The TUserDefaults class gives access to all user-customisable defaults.
    ///
    /// @Comment The User Defaults are stored in the Database (s_user_defaults table)
    ///   on the Server.
    /// </summary>
    public class TUserDefaults
    {
        /// <summary>
        /// todoComment
        /// </summary>
        public class NamedDefaults
        {
            /// <summary>todoComment</summary>
            public const String WINDOW_POSITION_PREFIX = "WINDOW_POS_";

            /// <summary>todoComment</summary>
            public const String WINDOW_POSITION_AND_SIZE_PREFIX = "WINDOW_POS_AND_SIZE_";

            /// <summary>Key name for Esc Closes Screen</summary>
            public const String USERDEFAULT_ESC_CLOSES_SCREEN = "EscClosesScreen";

            /// <summary>Key name for saving window size/state/position</summary>
            public const String USERDEFAULT_SAVE_WINDOW_POS_AND_SIZE = "SaveWindowPosAndSize";

            /// <summary>Colour of the background of all SourceGrid DataGrid instances.</summary>
            public const String COLOUR_GRID_BACKGROUND = "COLOUR_GRID_BACKGROUND";

            /// <summary>Colour of the background of every cell and row of all SourceGrid DataGrid instances.</summary>
            public const String COLOUR_GRID_CELLBACKGROUND = "COLOUR_GRID_CELLBACKGROUND";

            /// <summary>Alternating background colour of all SourceGrid DataGrid instances.</summary>
            public const String COLOUR_GRID_ALTERNATE = "COLOUR_GRID_ALTERNATE";

            /// <summary>Colour of the Selection of all SourceGrid DataGrid instances.</summary>
            public const String COLOUR_GRID_SELECTION = "COLOUR_GRID_SELECTION";

            /// <summary>Colour of the Grid Lines of all SourceGrid DataGrid instances.</summary>
            public const String COLOUR_GRID_GRIDLINES = "COLOUR_GRID_GRIDLINES";

            /// <summary>Colour of the Filter panel background.</summary>
            public const String COLOUR_FILTER_PANEL = "COLOUR_FILTER_PANEL";

            /// <summary>Colour of the Find panel background.</summary>
            public const String COLOUR_FIND_PANEL = "COLOUR_FIND_PANEL";

            /// <summary>Module to open when the application starts</summary>
            public const String MODULE_TO_OPEN_AT_STARTUP = "STARTUP_MODULE";

            #region TUserDefaults.TNamedDefaults

            /// <summary>
            /// Updates and immediately saves the UserDefault for the 'Last Partner Used'.
            /// </summary>
            /// <param name="APartnerKey">Partner Key of the last used Partner</param>
            /// <param name="ALastPartnerUse">Specifies which 'Last Partner Used' setting should be
            /// updated
            /// </param>
            /// <param name="APartnerClass">Partner Class of the Partner (default=<see cref="TPartnerClass.FAMILY"/>).
            /// Only relevant if <paramref name="ALastPartnerUse"/> is <see cref="TLastPartnerUse.lpuMailroomPartner"/>.</param>
            /// <returns>void</returns>
            public static void SetLastPartnerWorkedWith(Int64 APartnerKey,
                TLastPartnerUse ALastPartnerUse,
                TPartnerClass APartnerClass = TPartnerClass.FAMILY)
            {
                switch (ALastPartnerUse)
                {
                    case TLastPartnerUse.lpuMailroomPartner:
                        TUserDefaults.SetDefault(TUserDefaults.USERDEFAULT_LASTPARTNERMAILROOM, (object)APartnerKey);

                        if (APartnerClass == TPartnerClass.PERSON)
                        {
                            TUserDefaults.SetDefault(TUserDefaults.USERDEFAULT_LASTPERSONPERSONNEL, (object)APartnerKey);
                        }

                        break;

                    case TLastPartnerUse.lpuPersonnelPerson:
                        TUserDefaults.SetDefault(TUserDefaults.USERDEFAULT_LASTPERSONPERSONNEL, (object)APartnerKey);
                        break;

                    case TLastPartnerUse.lpuPersonnelUnit:
                        TUserDefaults.SetDefault(TUserDefaults.USERDEFAULT_LASTUNITPERSONNEL, (object)APartnerKey);
                        break;

                    case TLastPartnerUse.lpuConferencePerson:
                        TUserDefaults.SetDefault(TUserDefaults.USERDEFAULT_LASTPERSONCONFERENCE, (object)APartnerKey);

                        // if partner is person then also set this for personnel module
                        TUserDefaults.SetDefault(TUserDefaults.USERDEFAULT_LASTPERSONPERSONNEL, (object)APartnerKey);
                        break;
                }
            }

            #endregion
        }

        private static SUserDefaultsTable UUserDefaultsDataTable;
        private static DataView UUserDefaults;
        private static Boolean FIsInitialised = false;

        /// <summary>
        /// todoComment
        /// </summary>
        public class TInternal
        {
            #region TInternal

            /// <summary>
            /// todoComment
            /// </summary>
            /// <param name="AKey"></param>
            /// <returns></returns>
            public static bool HasUserDefault(String AKey)
            {
                return UUserDefaults.Find(AKey) != -1;
            }

            /// <summary>
            /// todoComment
            /// </summary>
            /// <param name="AKey"></param>
            /// <param name="ADefaultValue"></param>
            /// <returns></returns>
            public static String GetUserDefault(String AKey, String ADefaultValue)
            {
                String ReturnValue;
                Int32 FoundInRow;

                ReturnValue = "";
                FoundInRow = UUserDefaults.Find(AKey);

                if (FoundInRow != -1)
                {
                    // User default found
                    ReturnValue = (UUserDefaults[FoundInRow][SUserDefaultsTable.GetDefaultValueDBName()]).ToString();
                }
                else
                {
                    // User default not found, return default value
                    ReturnValue = ADefaultValue;
                }

                return ReturnValue;
            }

            /// <summary>
            /// overload
            /// </summary>
            /// <param name="AKey"></param>
            /// <returns></returns>
            public static String GetUserDefault(String AKey)
            {
                return GetUserDefault(AKey, "");
            }

            /// <summary>
            /// todoComment
            /// </summary>
            /// <param name="AKey"></param>
            /// <param name="AValue"></param>
            public static void SetUserDefault(String AKey, String AValue)
            {
                Int32 FoundInRow;
                DataRowView Tmp;

                FoundInRow = UUserDefaults.Find(AKey);

                if (FoundInRow != -1)
                {
                    // User default found
                    if (AValue != UUserDefaults[FoundInRow][SUserDefaultsTable.GetDefaultValueDBName()].ToString())
                    {
                        // Update only if the value is actually different
                        UUserDefaults[FoundInRow][SUserDefaultsTable.GetDefaultValueDBName()] = AValue;
                    }
                }
                else
                {
                    // User default not found, add it to the user defaults table
                    Tmp = UUserDefaults.AddNew();
                    Tmp[SUserDefaultsTable.GetUserIdDBName()] = Ict.Petra.Shared.UserInfo.GUserInfo.UserID;
                    Tmp[SUserDefaultsTable.GetDefaultCodeDBName()] = AKey;
                    Tmp[SUserDefaultsTable.GetDefaultValueDBName()] = AValue;
                    Tmp.EndEdit();
                }
            }

            #endregion
        }


        /// <summary>
        /// ------------------------------------------------------------------------------
        /// Petra General User Default Constants
        /// </summary>
        public const String PETRA_DISPLAYMODULEBACKGRDPICTURE = "DisplayPicture";

        /// <summary>todoComment</summary>
        public const String MAINMENU_VIEWOPTIONS_VIEWTASKS = "viewoptions_viewtasks";

        /// <summary>todoComment</summary>
        public const String MAINMENU_VIEWOPTIONS_TILESIZE = "viewoptions_tilesize";

        /// <summary>todoComment</summary>
        public const String MAINMENU_VIEWOPTIONS_SINGLECLICKEXECUTION = "viewoptions_singleclickexecution";

        /*------------------------------------------------------------------------------
         *  Partner User Default Constants
         * -------------------------------------------------------------------------------*/

        /// <summary>todoComment</summary>
        public const String PARTNER_FINDOPTIONS_CRITERIAFIELDSLEFT = "findopt_criteriafieldsleft";

        /// <summary>todoComment</summary>
        public const String PARTNER_FINDOPTIONS_CRITERIAFIELDSRIGHT = "findopt_criteriafieldsright";

        /// <summary>todoComment</summary>
        public const String PARTNER_FINDOPTIONSBYBANKDETAILS_CRITERIAFIELDSLEFT = "findoptbank_criteriafieldsleft";

        /// <summary>todoComment</summary>
        public const String PARTNER_FINDOPTIONSBYBANKDETAILS_CRITERIAFIELDSRIGHT = "findoptbank_criteriafieldsright";

        /// <summary>todoComment</summary>
        public const String PARTNER_FINDOPTIONS_NUMBEROFRESULTROWS = "findopt_numberofresultrows";

        /// <summary>todoComment</summary>
        public const String PARTNER_FINDOPTIONS_SHOWMATCHBUTTONS = "findopt_showmatchbuttons";

        /// <summary>todoComment</summary>
        public const String PARTNER_FINDOPTIONS_FIELDSDEFAULTMATCHVALUES = "findopt_fieldsdefaultmatchvalues";

        /// <summary>todoComment</summary>
        public const String PARTNER_FINDOPTIONS_EXACTPARTNERKEYMATCHSEARCH = "findopt_exactpartnerkeymatchsearch";

        /// <summary>todoComment</summary>
        public const String PARTNER_FIND_PARTNERDETAILS_OPEN = "partnfindscr_partnerdetails_open";

        /// <summary>todoComment</summary>
        public const String PARTNER_FIND_PARTNERTASKS_OPEN = "partnfindscr_partnertasks_open";

        /// <summary>todoComment</summary>
        public const String PARTNER_FIND_CRIT_FINDCURRADDRONLY = "Find Curr Addr Only";

        /// <summary>todoComment</summary>
        public const String PARTNER_FIND_CRIT_PARTNERSTATUS = "Find Partner Status";

        /// <summary>todoComment</summary>
        public const String PARTNER_EDIT_UPPERPARTCOLLAPSED = "partneredit_upperpartcollapsed";

        /// <summary>todoComment</summary>
        public const String PARTNER_EDIT_MATCHSETTINGS_LEFT = "partneredit_matchsettings_left";

        /// <summary>todoComment</summary>
        public const String PARTNER_EDIT_MATCHSETTINGS_RIGHT = "partneredit_matchsettings_right";

        /// <summary>todoComment</summary>
        public const String PARTNER_ACQUISITIONCODE = "p_acquisition";

        /// <summary>todoComment</summary>
        public const String PARTNER_LANGUAGECODE = "p_language";

        /// <summary>todoComment</summary>
        public const String PARTNER_TIPS = "MPartner_TipsState";

        /// <summary>Name of the last created extract </summary>
        public const String PARTNER_EXTRAC_LAST_EXTRACT_NAME = "Extract";

        /// <summary>todoComment</summary>
        public const String PERSONNEL_APPLICATION_STATUS = "ApplicationStatus";

        /// <summary>
        /// ------------------------------------------------------------------------------
        /// Reporting User Default Constants
        /// </summary>
        public const String FINANCE_REPORTING_SHOWDIFFFINANCIALYEARSELECTION = "ShowDiffFinancialYearSelection";

        /// <summary>which plugin to use for importing bank statements</summary>
        public const String FINANCE_BANKIMPORT_PLUGIN = "BankImportPlugin";

        /// <summary>which bank account to use</summary>
        public const String FINANCE_BANKIMPORT_BANKACCOUNT = "BankImportBankAccount";

        // Put other User Default Constants here as well.

        /// <summary>todoComment</summary>
        public const String USERDEFAULT_LASTPARTNERMAILROOM = "MailroomLastPerson";

        /// <summary>todoComment</summary>
        public const String USERDEFAULT_LASTPERSONPERSONNEL = "PersonnelLastPerson";

        /// <summary>todoComment</summary>
        public const String USERDEFAULT_LASTUNITPERSONNEL = "PersonnelLastUnit";

        /// <summary>todoComment</summary>
        public const String USERDEFAULT_LASTPERSONCONFERENCE = "ConferenceLastPerson";


        /*------------------------------------------------------------------------------
         *  Finance User Default Constants
         * -------------------------------------------------------------------------------*/

        /// <summary>todoComment</summary>
        public const String FINANCE_DEFAULT_LEDGERNUMBER = "a_default_ledger_number_i";

        /// <summary>Whether or not the user should be alerted when a new gift is from a new donor</summary>
        public const String FINANCE_GIFT_NEW_DONOR_ALERT = "NewDonorWarning";

        /// <summary>Whether or not the user should be allowed to enter Donor key 0000000000</summary>
        public const String FINANCE_GIFT_DONOR_ZERO_IS_VALID = "DonorZeroIsValid";

        /// <summary>Whether or not the gift detail comment fields are included when auto-copying the donor's latest gift details</summary>
        public const String FINANCE_GIFT_AUTO_COPY_INCLUDE_MAILING_CODE = "GiftAutoCopyIncludeMailingCode";

        /// <summary>Whether or not the gift detail comment fields are included when auto-copying the donor's latest gift details</summary>
        public const String FINANCE_GIFT_AUTO_COPY_INCLUDE_COMMENTS = "SplitGiftCopyIncludeComments";

        /// <summary>Whether or not the user should be allowed to enter Recipient key 0000000000</summary>
        public const String FINANCE_GIFT_RECIPIENT_ZERO_IS_VALID = "RecipientZeroIsValid";

        /// <summary>Whether or not the Gift Batch screen should be auto saved when creating a new gifts or adding a gift detail</summary>
        public const String FINANCE_GIFT_AUTO_SAVE = "AutoSaveGiftScreen";

        /// <summary>Whether or not the Gift Batch screen warns of inactive values ALSO on posting</summary>
        public const String FINANCE_GIFT_WARN_OF_INACTIVE_VALUES_ON_POSTING = "InactiveValuesWarningOnGiftPosting";

        /// <summary>Whether or not the GL Batch screen warns of inactive values ALSO on posting</summary>
        public const String FINANCE_GL_WARN_OF_INACTIVE_VALUES_ON_POSTING = "InactiveValuesWarningOnGLPosting";

        /* Other Finance User Defaults are in StringHelper because they are used in Ict.Common */


        /*------------------------------------------------------------------------------
         *  Conference Default Constants
         * -------------------------------------------------------------------------------*/

        /// <summary>todoComment</summary>
        public const String CONFERENCE_LASTCONFERENCEWORKEDWITH = "LastConferenceWorkedWith";


        /// <summary>
        /// initialise static variables
        /// </summary>
        public static void InitUserDefaults()
        {
            TRemote.MSysMan.Maintenance.UserDefaults.WebConnectors.GetUserDefaults(Ict.Petra.Shared.UserInfo.GUserInfo.UserID,
                out UUserDefaultsDataTable);
            UUserDefaults = new DataView(UUserDefaultsDataTable);
            UUserDefaults.Sort = SUserDefaultsTable.GetDefaultCodeDBName();
            FIsInitialised = true;
        }

        /// <summary>False if there's no user yet</summary>
        public static Boolean IsInitialised
        {
            get
            {
                return FIsInitialised;
            }
        }

        /// <summary>
        /// Saves one specific changed User Default.
        ///
        /// </summary>
        /// <param name="AKey">The Key of the User Default that should get saved.</param>
        /// <returns>true if successful, false if not.
        /// </returns>
        public static Boolean SaveChangedUserDefault(String AKey)
        {
            Boolean ReturnValue;
            SUserDefaultsRow SubmittedUserDefaultRow;
            SUserDefaultsTable DesiredUserDefaultsDataTable;
            Int32 FoundInRow;

            ReturnValue = false;
            FoundInRow = UUserDefaults.Find(AKey);

            if (FoundInRow != -1)
            {
                // User default found
                // MessageBox.Show('Found changed User Default with RowState ''' + Enum(UUserDefaults.Item[FoundInRow].Row.RowState).ToString("G") + '''');
                if ((UUserDefaults[FoundInRow].Row.RowState == DataRowState.Modified)
                    || (UUserDefaults[FoundInRow].Row.RowState == DataRowState.Added))
                {
                    DesiredUserDefaultsDataTable = (SUserDefaultsTable)UUserDefaultsDataTable.Clone();
                    DesiredUserDefaultsDataTable.InitVars();
                    SubmittedUserDefaultRow = DesiredUserDefaultsDataTable.NewRowTyped(false);
                    SubmittedUserDefaultRow.ItemArray = UUserDefaults[FoundInRow].Row.ItemArray;
                    DesiredUserDefaultsDataTable.Rows.Add(SubmittedUserDefaultRow);

                    if (UUserDefaults[FoundInRow].Row.RowState == DataRowState.Modified)
                    {
                        // Mark row as no longer beeing new
                        SubmittedUserDefaultRow.AcceptChanges();

                        // Mark row as beeing changed
                        SubmittedUserDefaultRow.DefaultValue = SubmittedUserDefaultRow.DefaultValue + ' ';
                        SubmittedUserDefaultRow.AcceptChanges();
                        SubmittedUserDefaultRow.DefaultValue = SubmittedUserDefaultRow.DefaultValue.Substring(0,
                            SubmittedUserDefaultRow.DefaultValue.Length - 1);
                    }

                    // MessageBox.Show('Saving single User Default ''' + DesiredUserDefaultsDataTable.Rows[0].Item['s_default_code_c'].ToString + '''');
                    TRemote.MSysMan.Maintenance.UserDefaults.WebConnectors.SaveUserDefaults(Ict.Petra.Shared.UserInfo.GUserInfo.UserID,
                        ref DesiredUserDefaultsDataTable);

                    // Copy over ModificationId that was changed on the Server side!
                    UUserDefaults[FoundInRow].Row.ItemArray = DesiredUserDefaultsDataTable.Rows.Find(
                        new Object[] { UUserDefaults[FoundInRow][0], UUserDefaults[FoundInRow][1] }).ItemArray;

                    // Mark this User Default as saved (unchanged)
                    UUserDefaults[FoundInRow].Row.AcceptChanges();
                    ReturnValue = true;
                }
            }
            else
            {
                ReturnValue = false;
            }

            return ReturnValue;
        }

        /// <summary>
        /// Saves all changed User Defaults.
        /// </summary>
        public static void SaveChangedUserDefaults()
        {
            SUserDefaultsTable UserDefaultsDataTableChanges;

            UserDefaultsDataTableChanges = UUserDefaultsDataTable.GetChangesTyped();

            if (UserDefaultsDataTableChanges != null)
            {
                // MessageBox.Show('Changed/added User Defaults: ' + UserDefaultsDataTableChanges.Rows.Count.ToString);
                TRemote.MSysMan.Maintenance.UserDefaults.WebConnectors.SaveUserDefaults(Ict.Petra.Shared.UserInfo.GUserInfo.UserID,
                    ref UserDefaultsDataTableChanges);

                // TODO 1 oChristianK cUserDefaults / ModificationID : Copy the ModificationID into the Client's DataTable so that the PetraClient's ModificationID's of UserDefaults are the same than the ones of the PetraServer.
                UUserDefaultsDataTable.AcceptChanges();
            }
        }

        /// <summary>
        /// Refreshes a UserDefault in the local Cache that has been updated on the
        /// Server side or in the DB.
        ///
        /// @comment This is needed for the case where several PetraClient instances of
        /// the same Petra User are running and they are updating the same UserDefault.
        /// On detecting such a 'collision', the PetraServer queues a ClientTask to
        /// refresh the specific UserDefault in the local Cache, and the
        /// TClientTaskInstance.Execute background thread then invokes this function.
        ///
        /// </summary>
        /// <param name="AChangedUserDefaultCode">UserDefault Code to update</param>
        /// <param name="AChangedUserDefaultValue">Changed UserDefault Value</param>
        /// <param name="AChangedUserDefaultModId">ModificationID of the changed UserDefault
        /// DataRow
        /// </param>
        /// <param name="AClientID">The Client ID that requested the refreshing (-1 if none was specified).</param>
        /// <returns>void</returns>
        public static void RefreshCachedUserDefault(String AChangedUserDefaultCode, String AChangedUserDefaultValue,
            String AChangedUserDefaultModId, int AClientID)
        {
            // Split String into String Array
            String[] ChangedUserDefaultCodes = AChangedUserDefaultCode.Split(new Char[] { RemotingConstants.GCLIENTTASKPARAMETER_SEPARATOR[0] });
            String[] ChangedUserDefaultValues = AChangedUserDefaultValue.Split(new Char[] { RemotingConstants.GCLIENTTASKPARAMETER_SEPARATOR[0] });
            String[] ChangedUserDefaultModIds = AChangedUserDefaultModId.Split(new Char[] { RemotingConstants.GCLIENTTASKPARAMETER_SEPARATOR[0] });

//            TLogging.Log("Refreshing User Default with DefaultCode '" + AChangedUserDefaultCode + "' with Value: '" +
//                AChangedUserDefaultValue + "'");

            if ((AClientID == -1)
                || (AClientID == UserInfo.GUserInfo.ProcessID))
            {
                for (Int16 Counter = 0; Counter <= ChangedUserDefaultCodes.Length - 1; Counter += 1)
                {
//                    TLogging.Log("Refreshing single UserDefault '" + ChangedUserDefaultCodes[Counter].ToString() + "' with value '" +
//                         ChangedUserDefaultValues[Counter] + "' (ModificationID: '" +
//                         ChangedUserDefaultModIds[Counter].ToString() + "'  (was sent from the same user session of this user on server)");
                    Int32 FoundInRow = UUserDefaults.Find(ChangedUserDefaultCodes[Counter]);

                    if (FoundInRow != -1)
                    {
                        // User default found
                        // TLogging.Log('Existing UserDefault ''' +
                        // UUserDefaults.Item[FoundInRow].Item[SUserDefaultsTable.GetDefaultCodeDBName].ToString + ''' with value ''' +
                        // UUserDefaults.Item[FoundInRow].Item[SUserDefaultsTable.GetDefaultValueDBName].ToString + ''' (ModificationID: ''' +
                        // UUserDefaults.Item[FoundInRow].Item[SUserDefaultsTable.GetModificationIDDBName].ToString + '''');
                        if (ChangedUserDefaultValues[Counter] != UUserDefaults[FoundInRow][SUserDefaultsTable.GetDefaultValueDBName()].ToString())
                        {
                            // Update only if the value is actually different
                            UUserDefaults[FoundInRow][SUserDefaultsTable.GetDefaultValueDBName()] = ChangedUserDefaultValues[Counter];
                        }

                        UUserDefaults[FoundInRow][SUserDefaultsTable.GetModificationIdDBName()] = Convert.ToDateTime(
                            ChangedUserDefaultModIds[Counter]);

                        // Mark this refreshed UserDefault as unchanged
                        UUserDefaults[FoundInRow].Row.AcceptChanges();
                    }
                    else
                    {
                        // User default not found, add it to the user defaults table
                        // TLogging.Log('UserDefault doesn''t exist yet > creating new one');
                        DataRowView Tmp = UUserDefaults.AddNew();
                        Tmp[SUserDefaultsTable.GetUserIdDBName()] = Ict.Petra.Shared.UserInfo.GUserInfo.UserID;
                        Tmp[SUserDefaultsTable.GetDefaultCodeDBName()] = ChangedUserDefaultCodes[Counter];
                        Tmp[SUserDefaultsTable.GetDefaultValueDBName()] = ChangedUserDefaultValues[Counter];
                        Tmp[SUserDefaultsTable.GetModificationIdDBName()] = ChangedUserDefaultModIds[Counter];
                        Tmp.EndEdit();

                        // Mark this refreshed UserDefault as unchanged
                        Tmp.Row.AcceptChanges();

                        // TLogging.Log('UserDefault: new Row added, RowState: ' + Enum(Tmp.Row.RowState).ToString("G"));
                    }
                }
            }
            else
            {
//                TLogging.Log("Refreshing ALL UserDefaults because the request for the refreshing of SINGLE user default was sent " +
//                    "from a DIFFERENT user session of THIS user on the server [hence we need to first update the server-side-held " +
//                    "UserDefaults for THIS session of the same user, and then the client-side-held UserDefaults, too])");
                ReloadCachedUserDefaultsOnServerAndClient();
            }
        }

        /// <summary>
        /// Reload the User Defaults.
        /// Causes TUserDefaults to reload the cached User Defaults Table
        ///
        /// </summary>
        /// <returns>void</returns>
        public static void ReloadCachedUserDefaults()
        {
//            TLogging.Log("ReloadCachedUserDefaults got called");
            SUserDefaultsTable TempUserDefaultsDataTable;
            DataSet UserDefaultsDS;

            // TODO 1 : ReaderWriterLock
            // reload user defaults from server
            TRemote.MSysMan.Maintenance.UserDefaults.WebConnectors.GetUserDefaults(Ict.Petra.Shared.UserInfo.GUserInfo.UserID,
                out TempUserDefaultsDataTable);

            // merge the current table with the one requested from the server so that client changes are not lost
            UserDefaultsDS = new DataSet();
            UserDefaultsDS.Tables.Add(UUserDefaultsDataTable);
            UserDefaultsDS.Merge((DataTable)TempUserDefaultsDataTable, true, MissingSchemaAction.Ignore);
            UUserDefaultsDataTable = (SUserDefaultsTable)UserDefaultsDS.Tables[SUserDefaultsTable.GetTableName()];
            UUserDefaults = new DataView(UUserDefaultsDataTable);
            UUserDefaults.Sort = SUserDefaultsTable.GetDefaultCodeDBName();
            UserDefaultsDS.Tables.Remove(UUserDefaultsDataTable);

            // remove table again (otherwise problems when called a second time)
        }

        /// <summary>
        /// Update the server-side-held UserDefaults for this session of the same user and then the client-side-held
        /// UserDefaults, too.
        /// </summary>
        public static void ReloadCachedUserDefaultsOnServerAndClient()
        {
//            TLogging.Log("ReloadCachedUserDefaultsOnServerAndClient got called");
            // First update the server-side-held UserDefaults for this session of the same user...
            TRemote.MSysMan.Maintenance.UserDefaults.WebConnectors.ReloadUserDefaults(UserInfo.GUserInfo.UserID,
                out UUserDefaultsDataTable);

            // ...then the client-side-held UserDefaults, too!
            UUserDefaults = new DataView(UUserDefaultsDataTable);
            UUserDefaults.Sort = SUserDefaultsTable.GetDefaultCodeDBName();
        }

        #region UserDefault Get and Set functions

        /// <summary>
        /// Find out if a user default exists already.
        /// This is required where the default should be calculated otherwise
        /// (e.g. FINANCE_REPORTING_SHOWDIFFFINANCIALYEARSELECTION)
        /// </summary>
        /// <returns>true if a default with the given key already exists
        /// </returns>
        public static bool HasDefault(String AKey)
        {
            return TInternal.HasUserDefault(AKey);
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <param name="ADefault"></param>
        /// <returns></returns>
        public static bool GetBooleanDefault(String AKey, bool ADefault)
        {
            bool ReturnValue;
            TVariant value;

            try
            {
                value = new TVariant(TInternal.GetUserDefault(AKey, ADefault.ToString()));
                ReturnValue = value.ToBool();
            }
            catch (Exception)
            {
                ReturnValue = false;
            }
            return ReturnValue;
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <returns></returns>
        public static bool GetBooleanDefault(String AKey)
        {
            return GetBooleanDefault(AKey, true);
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <param name="ADefault"></param>
        /// <returns></returns>
        public static System.Char GetCharDefault(String AKey, System.Char ADefault)
        {
            return Convert.ToChar(TInternal.GetUserDefault(AKey, ADefault.ToString()));
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <returns></returns>
        public static System.Char GetCharDefault(String AKey)
        {
            return GetCharDefault(AKey, ' ');
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <param name="ADefault"></param>
        /// <returns></returns>
        public static double GetDoubleDefault(String AKey, double ADefault)
        {
            return Convert.ToDouble(TInternal.GetUserDefault(AKey, ADefault.ToString()));
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <returns></returns>
        public static double GetDoubleDefault(String AKey)
        {
            return GetDoubleDefault(AKey, 0.0);
        }

        /// <summary>
        /// The following set of functions serve as shortcuts to get User Defaults of a
        /// specific type.
        ///
        /// </summary>
        /// <param name="AKey">The Key of the User Default that should get retrieved.</param>
        /// <param name="ADefault">The value that should be returned in case the Key is not (yet)
        /// in the User Defaults.
        /// </param>
        /// <returns>void</returns>
        public static System.Int16 GetInt16Default(String AKey, System.Int16 ADefault)
        {
            return Convert.ToInt16(TInternal.GetUserDefault(AKey, ADefault.ToString()));
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <returns></returns>
        public static System.Int16 GetInt16Default(String AKey)
        {
            return GetInt16Default(AKey, 0);
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <param name="ADefault"></param>
        /// <returns></returns>
        public static System.Int32 GetInt32Default(String AKey, System.Int32 ADefault)
        {
            return Convert.ToInt32(TInternal.GetUserDefault(AKey, ADefault.ToString()));
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <returns></returns>
        public static System.Int32 GetInt32Default(String AKey)
        {
            return GetInt32Default(AKey, 0);
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <param name="ADefault"></param>
        /// <returns></returns>
        public static System.Int64 GetInt64Default(String AKey, System.Int64 ADefault)
        {
            return Convert.ToInt64(TInternal.GetUserDefault(AKey, ADefault.ToString()));
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <returns></returns>
        public static System.Int64 GetInt64Default(String AKey)
        {
            return GetInt64Default(AKey, 0);
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <param name="ADefault"></param>
        /// <returns></returns>
        public static String GetStringDefault(String AKey, String ADefault)
        {
            return TInternal.GetUserDefault(AKey, ADefault);
        }

        /// <summary>
        /// todoComment
        /// </summary>
        /// <param name="AKey"></param>
        /// <returns></returns>
        public static String GetStringDefault(String AKey)
        {
            return GetStringDefault(AKey, "");
        }

        /// <summary>
        /// Sets a User Default.
        ///
        /// </summary>
        /// <param name="AKey">The Key of the User Default that should get saved.</param>
        /// <param name="AValue">The value that should be stored. Note: This can be anything on
        /// which ToString can be called.
        /// </param>
        /// <returns>void</returns>
        public static void SetDefault(String AKey, object AValue)
        {
            TInternal.SetUserDefault(AKey, AValue.ToString());
        }

        #endregion
    }
}