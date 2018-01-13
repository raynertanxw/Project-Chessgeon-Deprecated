using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaburuTools
{
	public struct DTJob
	{
		public delegate void OnCompleteCallback();
		// NOTE: Can be Async or non-async, but MUST execute onJobComplete.
		public delegate void BeginDelegate(OnCompleteCallback onJobComplete);

		internal readonly BeginDelegate Begin;
		internal readonly DTJob[] Dependencies;

		public DTJob(BeginDelegate inBegin, params DTJob[] inDependencies)
		{
			Begin = inBegin;
			Dependencies = inDependencies;
		}
	}

	public sealed class DTJobList
	{
		public DTJobList(DTJob.OnCompleteCallback inOnAllJobsComplete, params DTJob[] inJobs)
		{
			_allJobsCompleteCallback = inOnAllJobsComplete;
			_remainingJobs = new List<DTJob>(inJobs);
			foreach (DTJob job in inJobs)
			{
				TryBeginJob(job);
			}
		}

		private bool _hasStartedExecutingAllJobs = false;
		public void ExecuteAllJobs()
		{
			if (!_hasStartedExecutingAllJobs)
			{
				_hasStartedExecutingAllJobs = true;
				for (int iJob = 0; iJob < _remainingJobs.Count; iJob++)
				{
					TryBeginJob(_remainingJobs[iJob]);
				}
			}
		}

		// NOTE: If _jobsCompletionStatus does not have key, means dependency has not yet begun.
		private readonly Dictionary<DTJob, bool> _jobsCompletionStatus = new Dictionary<DTJob, bool>();
		private readonly Dictionary<DTJob, List<DTJob>> _dependents = new Dictionary<DTJob, List<DTJob>>();
		private readonly List<DTJob> _remainingJobs;
		private readonly DTJob.OnCompleteCallback _allJobsCompleteCallback;

		private void TryBeginJob(DTJob inJob)
		{
			bool jobStarted = _jobsCompletionStatus.ContainsKey(inJob);
			if (!jobStarted)
			{
				bool shouldBeginJob = true;
				DTJob[] dependencies = inJob.Dependencies;
				foreach (DTJob dependency in dependencies)
				{
					bool dependencyCompleted = false;
					bool dependencyStarted = _jobsCompletionStatus.TryGetValue(dependency, out dependencyCompleted);
					if (!dependencyCompleted)
					{
						shouldBeginJob = false;
						List<DTJob> dependents;
						if (!_dependents.TryGetValue(dependency, out dependents))
						{
							dependents = new List<DTJob>();
							_dependents.Add(dependency, dependents);
						}

						if (!dependents.Contains(inJob))
						{
							dependents.Add(inJob);
						}

						if (!dependencyStarted)
						{
							TryBeginJob(dependency);
						}
					}
				}

				bool jobComplete = false;
				if (shouldBeginJob)
				{
					if (!_jobsCompletionStatus.TryGetValue(inJob, out jobComplete))
					{
						_jobsCompletionStatus.Add(inJob, false);
					}

					if (!_remainingJobs.Contains(inJob))
					{
						_remainingJobs.Add(inJob);
					}

					if (!jobComplete)
					{
						inJob.Begin(() => OnJobCompleteHandler(inJob));
					}
				}
			}
		}

		private void OnJobCompleteHandler(DTJob inNewlyCompletedJob)
		{
			bool keyFound = _jobsCompletionStatus.ContainsKey(inNewlyCompletedJob);
			if (keyFound)
			{
				Debug.Assert(!_jobsCompletionStatus[inNewlyCompletedJob], "Init callback called more than once for " + inNewlyCompletedJob);
				_jobsCompletionStatus[inNewlyCompletedJob] = true;

				bool removed = _remainingJobs.Remove(inNewlyCompletedJob);
				Debug.Assert(removed, "Could not find " + inNewlyCompletedJob + " in _remainingInitialisables.");
				if (_remainingJobs.Count == 0)
				{
					if (_allJobsCompleteCallback != null)
					{
						_allJobsCompleteCallback();
					}
				}
			}
			else
			{
				Debug.LogError(inNewlyCompletedJob + " was not registered properly.");
			}
			List<DTJob> dependents;
			if (_dependents.TryGetValue(inNewlyCompletedJob, out dependents))
			{
				foreach (DTJob dependent in dependents)
				{
					TryBeginJob(dependent);
				}
			}
		}
	}
}
