using Args;

namespace WAWSDeploy
{
	/// <summary>
	/// Arguments used for publishing
	/// </summary>
	public class DeploymentArgs
	{
		/// <summary>
		/// Gets or sets the folder.
		/// </summary>
		/// <value>The folder.</value>
		[ArgsMemberSwitch(0)]
		public string Folder { get; set; }

		/// <summary>
		/// Gets or sets the publish settings file.
		/// </summary>
		/// <value>The publish settings file.</value>
		[ArgsMemberSwitch(1)]
		public string PublishSettingsFile { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[ArgsMemberSwitch("p", "password", "pw")]
		public string Password { get; set; }
	}
}
