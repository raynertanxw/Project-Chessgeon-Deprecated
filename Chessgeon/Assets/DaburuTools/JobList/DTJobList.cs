using System.Collections;
using System.Collections.Generic;

namespace DaburuTools
{
	public delegate void OnJobComplete();
	public delegate void Begin(OnJobComplete OnComplete);

	public struct DTJob
	{
		public DTJob(Begin inBegin, params DTJob[] inDependencies)
		{
			begin = inBegin;
			dependencies = inDependencies;
			numCompletedDependencies = 0;
		}
		internal readonly Begin begin;
		internal readonly DTJob[] dependencies;
		internal int numCompletedDependencies;

		internal int numDependencies { get { return dependencies.Length; } }
	}

	public class DTJobList
	{
		private bool _hasStartedExecutingJobs = false;
		private int _numSequencedJobsCompleted = 0;
		private readonly int _totalNumJobs;

		private OnJobComplete _onAllSequencedJobsComplete;
		private DTJob[] _jobs;

		public DTJobList(OnJobComplete OnComplete, params DTJob[] inJobs)
		{
			_hasStartedExecutingJobs = false;
			_numSequencedJobsCompleted = 0;
			_totalNumJobs = inJobs.Length;

			_onAllSequencedJobsComplete = OnComplete;
			_jobs = inJobs;
		}

		public void ExecuteAllJobs()
		{
			if (!_hasStartedExecutingJobs)
			{
				_hasStartedExecutingJobs = true;
				for (int iJob = 0; iJob < _totalNumJobs; iJob++)
				{
					ExecuteJob(_jobs[iJob], CheckIfAllJobsCompleted);
				}
			}
		}

		private void CheckIfAllJobsCompleted()
		{
			_numSequencedJobsCompleted++;
			if (_numSequencedJobsCompleted == _totalNumJobs)
			{
				if (_onAllSequencedJobsComplete != null) _onAllSequencedJobsComplete();
			}
		}

		private void ExecuteJob(DTJob inJob, OnJobComplete OnComplete)
		{
            if (inJob.numDependencies == 0 ||
				inJob.numCompletedDependencies == inJob.numDependencies)
			{
				inJob.begin(OnComplete);
			}
			else
			{
				ExecuteJob(inJob.dependencies[inJob.numCompletedDependencies], () => {
					inJob.numCompletedDependencies++;
					ExecuteJob(inJob, OnComplete);
				});
			}
		}
	}
}
