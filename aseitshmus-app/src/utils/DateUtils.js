// gets current date to be used to restrict date calendar
export function getCurrentDate() {
    const today = new Date();
    const day = String(today.getDate()).padStart(2, '0');
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const year = today.getFullYear();
    return `${day}-${month}-${year}`;
  }

  //Parses the date for the current date
  export function parseDate(dateString) {
    const parts = dateString.split('-');
    return new Date(parts[2], parts[1] - 1, parts[0]); 
  }


  const dateFormat = {
    day: "2-digit",
    month: "2-digit",
    year: "numeric"
};

//Transform date into string with format DD/MM/YYYY
export function getDate (value, format = "es-ES")  {
if (value !== null) {
  return new Date(value).toLocaleDateString(format, dateFormat).split('/').join('-');
} else {
    return "N/A";
}
}

//validate the date range
export function dateOrderValidator (from, thru) {
    return from <= thru;
}
