using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TfsUtils
{
    public class Utils
    {
        VsCredsWithUrl _VsCredsWithUrl = new VsCredsWithUrl();

        public void SetVsCredsWithUrl(string UserName, string Password, string Url)
        {
            _VsCredsWithUrl.UserName = UserName;
            _VsCredsWithUrl.Password = Password;
            _VsCredsWithUrl.Url = Url;
        }

        public void SetVsCredsWithUrl(VsCredsWithUrl _VsCredsWithUrlParam)
        {
            _VsCredsWithUrl = _VsCredsWithUrlParam;
        }

        public class SimpleWorkspace
        {
            public string Name { get; set; }
            public string Folder { get; set; }
        }

        public List<SimpleWorkspace> getAllWorkSpaceNames()
        {
            

            if (_VsCredsWithUrl == null)
                throw new System.InvalidOperationException("No Creds provided");


            VssCredentials vssCred = getVssCredentials(_VsCredsWithUrl.UserName, _VsCredsWithUrl.Password);

            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(_VsCredsWithUrl.Url), vssCred);
            tfs.Authenticate();
            // tfs server url including the  Collection Name --  CollectionName as the existing name of the collection from the tfs server 
            tfs.EnsureAuthenticated();

            VersionControlServer sourceControl = tfs.GetService<VersionControlServer>();
            //var workspace = sourceControl.CreateWorkspace("TestWorkspace", tfs.AuthorizedIdentity.UniqueName, "Temporary workspace for file retrieval");
            // in this case, "DEMO_Workspace"
            //var workspace = sourceControl.GetWorkspace("TestWorkspace", tfs.AuthorizedIdentity.UniqueName);
            Workspace[] retVal = sourceControl.QueryWorkspaces(null, tfs.AuthorizedIdentity.UniqueName, Environment.MachineName);


            //Newtonsoft.Json.JsonConvert.SerializeObject(retVal).Dump();



            //retVal.Dump();
            var _list = retVal.Select(x => new SimpleWorkspace { Name = x.Name, Folder = x.Folders.Select(f => f.LocalItem).FirstOrDefault() }).ToList();
            return _list;
        }

        public List<string> getAllProjects()
        {

            VssCredentials vssCred = getVssCredentials(_VsCredsWithUrl.UserName, _VsCredsWithUrl.Password);

            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(_VsCredsWithUrl.Url), vssCred);
            tfs.Authenticate();
            // tfs server url including the  Collection Name --  CollectionName as the existing name of the collection from the tfs server 
            tfs.EnsureAuthenticated();

            ICommonStructureService structureService = null;
            try
            {

                structureService =
                    (ICommonStructureService)tfs.GetService(typeof(ICommonStructureService));
            }
            catch (Exception e)
            {
                //ApplicationLogger.Log(e);
            }

            var projectInfoList = new List<ProjectInfo>(structureService.ListAllProjects());
            IEnumerable<string> data = projectInfoList.Select(proj => proj.Name);
            List<string> list = data.ToList();
            return list;
        }

        public bool getLatestVersion(string WorkSpaceName, string ProjectName)
        {
            //string teamProjectCollectionUrl = "https://bsipergemstar.visualstudio.com";

            VssCredentials vssCred = getVssCredentials(_VsCredsWithUrl.UserName, _VsCredsWithUrl.Password);

            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(_VsCredsWithUrl.Url), vssCred);
            tfs.Authenticate();
            // tfs server url including the  Collection Name --  CollectionName as the existing name of the collection from the tfs server 
            tfs.EnsureAuthenticated();

            VersionControlServer sourceControl = tfs.GetService<VersionControlServer>();
            //var workspace = sourceControl.CreateWorkspace("TestWorkspace", tfs.AuthorizedIdentity.UniqueName, "Temporary workspace for file retrieval");
            // in this case, "DEMO_Workspace"
            //var workspace = sourceControl.GetWorkspace("TestWorkspace", tfs.AuthorizedIdentity.UniqueName);
            var workspace = sourceControl.GetWorkspace(WorkSpaceName, tfs.AuthorizedIdentity.UniqueName);

            // For this workspace, map a server folder to a local folder
            //workspace.Map("$/Gemstar TFVC", @"C:\Users\bsiper.GEMSTARNY\Source\Workspaces\GemstarTFVCProgramatic");

            var fileRequest = new GetRequest(
                //new ItemSpec("$/Gemstar TFVC", RecursionType.Full), VersionSpec.Latest);
                new ItemSpec(ProjectName, RecursionType.Full), VersionSpec.Latest);
            // Get latest
            var results = workspace.Get(fileRequest, GetOptions.Overwrite | GetOptions.Overwrite);

            //var ws = sourceControl.GetWorkspace(@"C:\Users\bsiper.GEMSTARNY\Source\Workspaces\Gemstar TFVC2");
            //var wss = sourceControl.QueryWorkspaces(null, "bsiper", Environment.MachineName);


            //sourceControl.CreateWorkspace(
            //results.Dump();

            //Workspace[] workspaces = sourceControl.QueryWorkspaces(workspaceName, sourceControl.AuthenticatedUser, Workstation.Current.Name);
            //Workspace[] workspaces = sourceControl.QueryWorkspaces(

            return true;
        }

        public VssCredentials getVssCredentials(string UserName, string Password)
        {
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            VssBasicCredential bsCred = new VssBasicCredential(netCred);
            VssCredentials vssCred = new VssClientCredentials(bsCred);

            return vssCred;
        }



        public class VsCredsWithUrl
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Url { get; set; }
        }
    }
}
