using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TaskRegistry
{
	public delegate Task CreateTaskDelegate(TaskManager taskManager, TaskManager.Priority priority, Task.Config flags, TaskDescriptor descriptor);

	private Dictionary<string, CreateTaskDelegate> m_Tasks;

	private static TaskRegistry m_instance;

	public IEnumerable<string> GetTaskNames()
	{
		return m_Tasks.Keys;
	}

	public Task CreateTask(TaskManager taskManager, TaskManager.Priority priority, Task.Config flags, TaskDescriptor descriptor)
	{
		return descriptor.CreateTask(taskManager, priority, flags);
	}

	private void Initialise()
	{
		m_Tasks = GetTasks();
	}

	public static TaskRegistry Instance()
	{
		if (m_instance == null)
		{
			m_instance = new TaskRegistry();
			m_instance.Initialise();
		}
		return m_instance;
	}

	private static Dictionary<string, CreateTaskDelegate> GetTasks()
	{
		Dictionary<string, CreateTaskDelegate> dictionary = new Dictionary<string, CreateTaskDelegate>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsAbstract || !typeof(Task).IsAssignableFrom(type))
				{
					continue;
				}
				string name = "CreateTask";
				BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public;
				Binder defaultBinder = Type.DefaultBinder;
				Type[] types2 = new Type[4]
				{
					typeof(TaskManager),
					typeof(TaskManager.Priority),
					typeof(Task.Config),
					typeof(TaskDescriptor)
				};
				ParameterModifier[] modifiers = null;
				MethodInfo methodInfo = type.GetMethod(name, bindingAttr, defaultBinder, types2, modifiers);
				if (methodInfo != null && methodInfo.ReturnType == typeof(Task))
				{
					dictionary[type.Name] = (TaskManager taskManager, TaskManager.Priority priority, Task.Config flags, TaskDescriptor descriptor) => methodInfo.Invoke(null, new object[4] { taskManager, priority, flags, descriptor }) as Task;
				}
				else
				{
					Debug.LogWarning(string.Format("Task '{0}' has missing or invalid CreateTask method", type.Name));
				}
			}
		}
		return dictionary;
	}
}
