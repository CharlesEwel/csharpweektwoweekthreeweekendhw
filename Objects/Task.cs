using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace ToDoList.Objects
{
  public class Task
  {
    private int _id;
    private string _description;
    private DateTime _dueDate;
    private int _categoryId;

    public Task(string Description, DateTime DueDate, int CategoryId, int Id = 0)
    {
      _id = Id;
      _description = Description;
      _dueDate = DueDate;
      _categoryId = CategoryId;
    }

    public override bool Equals(System.Object otherTask)
    {
      if(!(otherTask is Task))
      {
        return false;
      }
      else
      {
        Task newTask = (Task) otherTask;
        bool idEquality = (this.GetId() == newTask.GetId());
        bool descriptionEquality = (this.GetDescription()== newTask.GetDescription());
        bool dueDateEquality = this.GetDueDate() == newTask.GetDueDate();
        bool categoryEquality = this.GetCategoryId() == newTask.GetCategoryId();
        return (idEquality && descriptionEquality && categoryEquality && dueDateEquality);
      }
    }

    public int GetId()
    {
      return _id;
    }

    public string GetDescription()
    {
      return _description;
    }

    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }

    public DateTime GetDueDate()
    {
      return _dueDate;
    }

    public void SetDueDate(DateTime newDueDate)
    {
      _dueDate = newDueDate;
    }
    public int GetCategoryId()
    {
      return _categoryId;
    }
    public void SetCategoryId(int newCategoryId)
    {
      _categoryId = newCategoryId;
    }
    public static List<Task> GetAll()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks ORDER BY due_date;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDescription = rdr.GetString(1);
        DateTime taskDueDate = rdr.GetDateTime(3);
        int taskCategoryId = rdr.GetInt32(2);
        Task newTask = new Task(taskDescription, taskDueDate, taskCategoryId, taskId);
        allTasks.Add(newTask);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allTasks;
    }



    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

       SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description, due_date, category_id) OUTPUT INSERTED.id VALUES (@TaskDescription, @TaskDueDate, @TaskCategoryId);", conn);

      SqlParameter descriptionParameter = new SqlParameter();
      descriptionParameter.ParameterName = "@TaskDescription";
      descriptionParameter.Value = this.GetDescription();

      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@TaskCategoryId";
      categoryIdParameter.Value = this.GetCategoryId();

      SqlParameter dueDateParameter = new SqlParameter();
      dueDateParameter.ParameterName = "@TaskDueDate";
      dueDateParameter.Value = this.GetDueDate();

      cmd.Parameters.Add(descriptionParameter);
      cmd.Parameters.Add(categoryIdParameter);
      cmd.Parameters.Add(dueDateParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM tasks;", conn);
      cmd.ExecuteNonQuery();
    }

    public static Task Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks WHERE id= @TaskId;", conn);
      SqlParameter taskIdParameter = new SqlParameter();
      taskIdParameter.ParameterName = "@TaskId";
      taskIdParameter.Value = id.ToString();
      cmd.Parameters.Add(taskIdParameter);
      rdr = cmd.ExecuteReader();

      int foundTaskId = 0;
      string foundTaskDescription = null;
      int foundTaskCategoryId = 0;
      DateTime foundTaskDueDate = new DateTime(1990, 09 ,05);

      while(rdr.Read())
      {
        foundTaskId = rdr.GetInt32(0);
        foundTaskDescription = rdr.GetString(1);
        foundTaskDueDate = rdr.GetDateTime(3);
        foundTaskCategoryId = rdr.GetInt32(2);
      }
      Task foundTask = new Task(foundTaskDescription, foundTaskDueDate, foundTaskCategoryId, foundTaskId);
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundTask;
    }
  }
}
