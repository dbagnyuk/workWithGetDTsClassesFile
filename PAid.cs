using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using workWithGetDTsClassesFile.ISca;

namespace workWithGetDTsClassesFile
{
    public class PAid
    {
        long personalAccount;
        List<TDid> tdsList = new List<TDid>();
        List<long> tdcWithLookedSrv = new List<long>();
        public void processInputData(long inputPA, GetTDsOutput inputData)
        {
            this.personalAccount = inputPA;

            foreach (var tdcs in inputData.TDs)
            {
                TDid oneTDid = new TDid();
                oneTDid.processInputData(tdcs.TdId, tdcs.Services);

                tdsList.Add(oneTDid);
            }
        }
        public void writePAtoConsole()
        {
            Console.WriteLine($"TDIds with Services for PA {this.personalAccount}:\n");
            this.tdsList.ForEach(i => i.writeTDtoConsole());
        }
        public void writePAtoFile(ref StreamWriter sw)
        {
            StreamWriter swPA = sw;
            swPA.WriteLine($"TDIds with Services for PA {this.personalAccount}:\n");
            this.tdsList.ForEach(i => i.writeTDtoFile(ref swPA));
        }
        public void writeTDwithSrvtoConsole(string lookedSrv)
        {
            if (!string.IsNullOrEmpty(lookedSrv))
            {
                Console.WriteLine($"\n\nThe TDs which have the valid service {lookedSrv}:");
                tdcWithLookedSrv.ForEach(i => Console.Write("{0}\n", i));
            }
        }
        public void writeTDwithSrvtoFile(string lookedSrv, ref StreamWriter sw)
        {
            StreamWriter swLookSrv = sw;
            if (!string.IsNullOrEmpty(lookedSrv))
            {
                swLookSrv.WriteLine($"\n\nThe TDs which have the valid service {lookedSrv}:");
                tdcWithLookedSrv.ForEach(i => swLookSrv.Write("{0}\n", i));
            }
        }
        public void lookupSrv(string lookedSrv)
        {
            if (!string.IsNullOrEmpty(lookedSrv))
            {
                foreach (TDid i in tdsList)
                {
                    if (i.lookupSrv(lookedSrv) != 0)
                        tdcWithLookedSrv.Add(i.lookupSrv(lookedSrv));
                }
            }
        }
        public void sortTDsList(string sortTDsType, string sortSrvsType)
        {
            if (sortTDsType == "asc")
            {
                tdsList = tdsList.OrderBy(p => p.returnTDid()).ToList();
                tdcWithLookedSrv = tdcWithLookedSrv.OrderBy(p => p).ToList();
                this.tdsList.ForEach(i => i.sortSrvList(sortSrvsType));
            }
            else if (sortTDsType == "desc")
            {
                tdsList = tdsList.OrderByDescending(p => p.returnTDid()).ToList();
                tdcWithLookedSrv = tdcWithLookedSrv.OrderByDescending(p => p).ToList();
                this.tdsList.ForEach(i => i.sortSrvList(sortSrvsType));
            }
        }
    }
}
