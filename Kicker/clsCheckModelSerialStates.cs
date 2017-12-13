using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Kicker
{
    public class clsCheckModelSerialStates
    {
        private string strSqlConnection1;

        public clsCheckModelSerialStates(string strConnectionString)
        {

            strSqlConnection1 = strConnectionString;
        }

        public string checkForPrePrint(string strModel,string strACSSerial, ref string strProdOrder, ref string strPrintType)
        {
            SqlConnection sqlConnectionACSEE ;
            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnection1))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdGetProdOrderInfo = sqlConnectionACSEE.CreateCommand();
                            cmdGetProdOrderInfo.CommandType = CommandType.StoredProcedure;
                            cmdGetProdOrderInfo.CommandText = "ame_TFFC_GetProdOrderInfo";


                            cmdGetProdOrderInfo.Parameters.Add("@model", SqlDbType.Char, 20);
                            cmdGetProdOrderInfo.Parameters["@model"].Value = strModel;
                            cmdGetProdOrderInfo.Parameters["@model"].Direction = ParameterDirection.Input;

                            cmdGetProdOrderInfo.Parameters.Add("@acsserial", SqlDbType.Char, 20);
                            cmdGetProdOrderInfo.Parameters["@acsserial"].Value = strACSSerial;
                            cmdGetProdOrderInfo.Parameters["@acsserial"].Direction = ParameterDirection.Input;


                            cmdGetProdOrderInfo.Parameters.Add("@prodorder", SqlDbType.Char, 20);
                            cmdGetProdOrderInfo.Parameters["@prodorder"].Direction = ParameterDirection.Output;

                            cmdGetProdOrderInfo.Parameters.Add("@printtype", SqlDbType.Char, 20);
                            cmdGetProdOrderInfo.Parameters["@printtype"].Direction = ParameterDirection.Output;



                            using (SqlDataReader rd = cmdGetProdOrderInfo.ExecuteReader())
                            {
                                if (rd.HasRows == true)
                                {
                                    rd.Read();
                                    strProdOrder = rd["ProductionOrder"].ToString();
                                    strPrintType = rd["PrintType"].ToString();
                                    return "OK";
                                }
                                else
                                {
                                    return "NOTFOUND";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strPrintType ;
        }


        public string ReserveSerialForProductionOrder(string strModel, string strACSSerial, 
            string strProductionOrder, string strStation, ref string strPSCSerial)
        {

            SqlConnection sqlConnectionACSEE ;
            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnection1))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdReservePSCSerial = sqlConnectionACSEE.CreateCommand();
                            cmdReservePSCSerial.CommandType = CommandType.StoredProcedure;
                            cmdReservePSCSerial.CommandText = "ame_TFFC_ReserveSerial";


                            cmdReservePSCSerial.Parameters.Add("@prodorder", SqlDbType.Char, 20);
                            cmdReservePSCSerial.Parameters["@prodorder"].Value = strProductionOrder ;
                            cmdReservePSCSerial.Parameters["@prodorder"].Direction = ParameterDirection.Input;


                            cmdReservePSCSerial.Parameters.Add("@acsserial", SqlDbType.Char, 20);
                            cmdReservePSCSerial.Parameters["@acsserial"].Value = strACSSerial;
                            cmdReservePSCSerial.Parameters["@acsserial"].Direction = ParameterDirection.Input;



                            cmdReservePSCSerial.Parameters.Add("@material", SqlDbType.Char, 20);
                            cmdReservePSCSerial.Parameters["@material"].Value = strModel;
                            cmdReservePSCSerial.Parameters["@material"].Direction = ParameterDirection.Input;

                            cmdReservePSCSerial.Parameters.Add("@station", SqlDbType.Char, 20);
                            cmdReservePSCSerial.Parameters["@station"].Value = strStation;
                            cmdReservePSCSerial.Parameters["@station"].Direction = ParameterDirection.Input;



                            cmdReservePSCSerial.Parameters.Add("@serial", SqlDbType.Char, 20);
                            cmdReservePSCSerial.Parameters["@serial"].Direction = ParameterDirection.Output;


                            using (SqlDataReader rd = cmdReservePSCSerial.ExecuteReader())
                            {
                                if (rd.HasRows == true)
                                {
                                    rd.Read();
                                    strPSCSerial = rd["serial"].ToString();
//                                    strPrintType = rd["PrintType"].ToString();
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return strPSCSerial;
        }


        public string GetModelbySerial( string strACSSerial           )
        {

            SqlConnection sqlConnectionACSEE;
            string strTempModel = "NoModel";
            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnection1))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdReservePSCSerial = sqlConnectionACSEE.CreateCommand();
                            cmdReservePSCSerial.CommandType = CommandType.StoredProcedure;
                            cmdReservePSCSerial.CommandText = "amevn_getmodelbyserial";



                            cmdReservePSCSerial.Parameters.Add("@Serial", SqlDbType.Char, 30);
                            cmdReservePSCSerial.Parameters["@Serial"].Value = strACSSerial;
                            cmdReservePSCSerial.Parameters["@Serial"].Direction = ParameterDirection.Input;


                            

                            using (SqlDataReader rd = cmdReservePSCSerial.ExecuteReader())
                            {
                                if (rd.HasRows == true)
                                {
                                    rd.Read();
                                    strTempModel = rd["SAP_Model"].ToString();
                                    //                                    strPrintType = rd["PrintType"].ToString();
                                }
                            }

                           


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return strTempModel;
            
        }


        public string UnReserveSerial(string strModel, string strACSSerial,
            string strProductionOrder, string strStation, ref string strPSCSerial)
        {

            SqlConnection sqlConnectionACSEE;
            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnection1))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdUnReservePSCSerial = sqlConnectionACSEE.CreateCommand();
                            cmdUnReservePSCSerial.CommandType = CommandType.StoredProcedure;
                            cmdUnReservePSCSerial.CommandText = "ame_TFFC_UnReserveSerial";


                            cmdUnReservePSCSerial.Parameters.Add("@prodorder", SqlDbType.Char, 20);
                            cmdUnReservePSCSerial.Parameters["@prodorder"].Value = strProductionOrder;
                            cmdUnReservePSCSerial.Parameters["@prodorder"].Direction = ParameterDirection.Input;


                            cmdUnReservePSCSerial.Parameters.Add("@acsserial", SqlDbType.Char, 20);
                            cmdUnReservePSCSerial.Parameters["@acsserial"].Value = strACSSerial;
                            cmdUnReservePSCSerial.Parameters["@acsserial"].Direction = ParameterDirection.Input;



                            cmdUnReservePSCSerial.Parameters.Add("@material", SqlDbType.Char, 20);
                            cmdUnReservePSCSerial.Parameters["@material"].Value = strModel;
                            cmdUnReservePSCSerial.Parameters["@material"].Direction = ParameterDirection.Input;


                            cmdUnReservePSCSerial.Parameters.Add("@serial", SqlDbType.Char, 20);
                            cmdUnReservePSCSerial.Parameters["@serial"].Value = strPSCSerial;
                            cmdUnReservePSCSerial.Parameters["@serial"].Direction = ParameterDirection.Input;

                            cmdUnReservePSCSerial.Parameters.Add("@success", SqlDbType.Char, 20);
                            cmdUnReservePSCSerial.Parameters["@success"].Direction = ParameterDirection.Output;


                            using (SqlDataReader rd = cmdUnReservePSCSerial.ExecuteReader())
                            {
/*
                                if (rd.HasRows == true)
                                {
                                    rd.Read();
                                    strPSCSerial = rd["serial"].ToString();
                                    //                                    strPrintType = rd["PrintType"].ToString();
                                }
 */
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return "OK" ;
        }


        public string CommitSerial(string strModel, string strACSSerial,
            string strProductionOrder, string strStation, string strPSCSerial, ref string strUsedPSCSerial)
        {

            SqlConnection sqlConnectionACSEE;
            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnection1))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdConsumeReservedSerial = sqlConnectionACSEE.CreateCommand();
                            cmdConsumeReservedSerial.CommandType = CommandType.StoredProcedure;
                            cmdConsumeReservedSerial.CommandText = "ame_TFFC_ConsumeReservedSerial";


                            cmdConsumeReservedSerial.Parameters.Add("@prodorder", SqlDbType.Char, 20);
                            cmdConsumeReservedSerial.Parameters["@prodorder"].Value = strProductionOrder;
                            cmdConsumeReservedSerial.Parameters["@prodorder"].Direction = ParameterDirection.Input;


                            cmdConsumeReservedSerial.Parameters.Add("@acsserial", SqlDbType.Char, 20);
                            cmdConsumeReservedSerial.Parameters["@acsserial"].Value = strACSSerial;
                            cmdConsumeReservedSerial.Parameters["@acsserial"].Direction = ParameterDirection.Input;



                            cmdConsumeReservedSerial.Parameters.Add("@material", SqlDbType.Char, 20);
                            cmdConsumeReservedSerial.Parameters["@material"].Value = strModel;
                            cmdConsumeReservedSerial.Parameters["@material"].Direction = ParameterDirection.Input;



                            cmdConsumeReservedSerial.Parameters.Add("@station", SqlDbType.Char, 20);
                            cmdConsumeReservedSerial.Parameters["@station"].Value = strStation;
                            cmdConsumeReservedSerial.Parameters["@station"].Direction = ParameterDirection.Input;



                            cmdConsumeReservedSerial.Parameters.Add("@usedserial", SqlDbType.Char, 20);
                            cmdConsumeReservedSerial.Parameters["@usedserial"].Value = strPSCSerial;
                            cmdConsumeReservedSerial.Parameters["@usedserial"].Direction = ParameterDirection.Input;

                            cmdConsumeReservedSerial.Parameters.Add("@serial", SqlDbType.Char, 20);
                            cmdConsumeReservedSerial.Parameters["@serial"].Direction = ParameterDirection.Output;


                            using (SqlDataReader rd = cmdConsumeReservedSerial.ExecuteReader())
                            {
                                /*
                                                                if (rd.HasRows == true)
                                                                {
                                                                    rd.Read();
                                                                    strPSCSerial = rd["serial"].ToString();
                                                                    //                                    strPrintType = rd["PrintType"].ToString();
                                                                }
                                 */
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return "OK";
        }

        public string UnCommitSerial(string strModel, string strACSSerial,
            string strProductionOrder, string strStation, ref string strPSCSerial)
        {

            SqlConnection sqlConnectionACSEE;
            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnection1))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdUnConsumeSerial = sqlConnectionACSEE.CreateCommand();
                            cmdUnConsumeSerial.CommandType = CommandType.StoredProcedure;
                            cmdUnConsumeSerial.CommandText = "ame_TFFC_UnConsumeSerial";


                            cmdUnConsumeSerial.Parameters.Add("@prodorder", SqlDbType.Char, 20);
                            cmdUnConsumeSerial.Parameters["@prodorder"].Value = strProductionOrder;
                            cmdUnConsumeSerial.Parameters["@prodorder"].Direction = ParameterDirection.Input;


                            cmdUnConsumeSerial.Parameters.Add("@acsserial", SqlDbType.Char, 20);
                            cmdUnConsumeSerial.Parameters["@acsserial"].Value = strACSSerial;
                            cmdUnConsumeSerial.Parameters["@acsserial"].Direction = ParameterDirection.Input;



                            cmdUnConsumeSerial.Parameters.Add("@material", SqlDbType.Char, 20);
                            cmdUnConsumeSerial.Parameters["@material"].Value = strModel;
                            cmdUnConsumeSerial.Parameters["@material"].Direction = ParameterDirection.Input;


                            cmdUnConsumeSerial.Parameters.Add("@serial", SqlDbType.Char, 20);
                            cmdUnConsumeSerial.Parameters["@serial"].Value = strPSCSerial;
                            cmdUnConsumeSerial.Parameters["@serial"].Direction = ParameterDirection.Input;

                            cmdUnConsumeSerial.Parameters.Add("@success", SqlDbType.Char, 20);
                            cmdUnConsumeSerial.Parameters["@success"].Direction = ParameterDirection.Output;


                            using (SqlDataReader rd = cmdUnConsumeSerial.ExecuteReader())
                            {
                                /*
                                                                if (rd.HasRows == true)
                                                                {
                                                                    rd.Read();
                                                                    strPSCSerial = rd["serial"].ToString();
                                                                    //                                    strPrintType = rd["PrintType"].ToString();
                                                                }
                                 */
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return "OK";
        }

        public string verifyFivedash(string strModel, string strACSSerial)
        {
            return "P";
        }


        public string CheckForTestFinish(string strModel, string strACSSerial, string strPSCSerial)
        {
            return "";
        }
    }
}
