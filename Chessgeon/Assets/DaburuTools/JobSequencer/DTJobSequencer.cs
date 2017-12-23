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

	public class DTJobSequencer
	{
		private bool _hasStartedExecutingJobs = false;
		private int _numSequencedJobsCompleted = 0;
		private readonly int _totalNumSequencedJobs;

		private OnJobComplete _onAllSequencedJobsComplete;
		private DTJob[] _jobs;

		public DTJobSequencer(OnJobComplete OnComplete, params DTJob[] inJobs)
		{
			_hasStartedExecutingJobs = false;
			_numSequencedJobsCompleted = 0;
			_totalNumSequencedJobs = inJobs.Length;

			_onAllSequencedJobsComplete = OnComplete;
			_jobs = inJobs;
		}

		public void ExecuteJobSequence()
		{
			if (!_hasStartedExecutingJobs)
			{
				_hasStartedExecutingJobs = true;
				ExecuteSequencedJob(_jobs[_numSequencedJobsCompleted]);
			}
		}

		private void FinishJobSequence()
		{
            if (_onAllSequencedJobsComplete != null) _onAllSequencedJobsComplete();
		}

		private void ExecuteSequencedJob(DTJob inJob)
		{
            if (inJob.numDependencies == 0 ||
				inJob.numCompletedDependencies == inJob.numDependencies)
			{
				inJob.begin(() => {
					_numSequencedJobsCompleted++;

					if (_numSequencedJobsCompleted == _totalNumSequencedJobs) FinishJobSequence();
					else ExecuteSequencedJob(_jobs[_numSequencedJobsCompleted]);
				});
			}
			else
			{
				ExecuteDependency(inJob.dependencies[inJob.numCompletedDependencies], () => {
					inJob.numCompletedDependencies++;
					ExecuteSequencedJob(inJob); });
			}
		}

		private void ExecuteDependency(DTJob inDependency, OnJobComplete onDependencyComplete)
		{
            if (inDependency.numDependencies == 0 ||
				inDependency.numCompletedDependencies == inDependency.numDependencies)
			{
				inDependency.begin(onDependencyComplete);
			}
			else
			{
				ExecuteDependency(inDependency.dependencies[inDependency.numCompletedDependencies], () => {
					inDependency.numCompletedDependencies++;
					ExecuteDependency(inDependency, onDependencyComplete);
				});
			}
		}
	}
}
