using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Windows;
using System.Collections.ObjectModel;

namespace Semiconductor_Equipment_Monitor
{
    public class EquipmentDataService
    {
        //MySQL 链接字符串
        private const string _connectionString =
            "server=localhost;" +
            "user=root;" +
            "password=123456;" +
            "database=WpfEquipment;" +
            "port=3306;" +
            "charset=utf8mb4";



        /// <summary>
        /// 保存数据至数据库
        /// </summary>
        public static  void SaveEquipment2MySql(ObservableCollection<Equipment> EquipmentList)
        {
            try
            {
                //1.创建数据库链接
                using (var conn = new MySqlConnection(_connectionString))
                {
                    //2.打开链接
                    conn.Open();
                    //3.清空旧数据（测试用，正式环境需删除）   命令 清空equipment表内数据
                    using (var truncateCmd = new MySqlCommand("TRUNCATE TABLE Equipment", conn))
                    {
                        //执行 sqlCommand 对象中定义的sql语句
                        truncateCmd.ExecuteNonQuery();
                    }

                    //4.循环插入各设备数据
                    foreach (var device in EquipmentList)
                    {
                        string insertSql = @"
                            INSERT INTO Equipment (
                                EqpId,
                                EqpName,
                                DeviceName,
                                Status,
                                LineId,
                                OutputToday
                                )
                            VALUES (
                                @EqpId, 
                                @EqpName,
                                @DeviceName,
                                @Status,
                                @LineID,
                                @OutputToday);";//@DeviceName,@Status占位符

                        using (var cmd = new MySqlCommand(insertSql, conn))
                        {
                            //给参数赋值，防止SQL注入风险
                            cmd.Parameters.AddWithValue("@EqpId", device.EqpId);
                            cmd.Parameters.AddWithValue("@EqpName", device.EqpName);
                            cmd.Parameters.AddWithValue("@DeviceName", device.EqpName);

                            cmd.Parameters.AddWithValue("@Status", device.Status.ToString());
                            cmd.Parameters.AddWithValue("@LineId", device.LineId);
                            cmd.Parameters.AddWithValue("@OutputToday", device.OutputToday);

                            //执行 插入 语句
                            cmd.ExecuteNonQuery();//主要用于 增 删 改 不需要要返回值的SQL语句
                        }

                    }
                }
                MessageBox.Show("✔ 设备数据已成功保存至MySQL！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {

                MessageBox.Show($"❌ 保存失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 从SQL获取数据
        /// </summary>
        /// <param name="EquipmentList"></param>
        public static void LoadFromMySql(ObservableCollection<Equipment> EquipmentList) 
        {
            try
            {
                EquipmentList.Clear();//清空数据列表
                using (var conn = new MySqlConnection(_connectionString)) 
                {
                    conn.Open();

                    string selectSql = @"
                        SELECT  EqpId,
                                EqpName,
                                DeviceName,
                                Status,
                                LineId,
                                OutputToday,
                                LastUpdateTime From Equipment; ";

                    using (var cmd = new MySqlCommand(selectSql, conn))//查
                    using (var reader = cmd.ExecuteReader()) //executereader 执行读取数据
                    {
                        while (reader.Read()) 
                        {
                            EquipmentList.Add(new Equipment 
                            {
                                EqpId = reader["EqpId"].ToString(),
                                EqpName = reader["EqpName"].ToString(),
                                DeviceName = reader["DeviceName"].ToString(),
                                Status = (EquipmentStatus)Enum.Parse(typeof(EquipmentStatus), reader["Status"].ToString()),
                                //string类型转换为枚举类型
                                LineId = reader["LineId"].ToString(),
                                OutputToday = Convert.ToInt32( reader["OutputToday"]),
                                LastUpdateTime = Convert.ToDateTime(reader["LastUpdateTime"])
                            });
                            
                        }
                    }
                }
                MessageBox.Show("✔ 成功从MySQL读取数据！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"❌ 读取失败：{ex.Message}","错误",MessageBoxButton.OK,MessageBoxImage.Error);  
            }
        }




    }
}
