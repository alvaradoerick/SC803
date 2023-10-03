

export function moveCursorToBeginning () {
    const inputElement = document.getElementById('mask-input');
    if (inputElement) {
        inputElement.setSelectionRange(0, 0);
    } 
}
export function moveCursorToBeginningForIBAN () {
  const inputElement = document.getElementById('IBAN-input');
  if (inputElement) {
      inputElement.setSelectionRange(2, 2);
  } 
}

export function statusName (statusValue) {
  return statusValue ? 'Activo' : 'Inactivo'
}


export function getRequiresAction (value) {
  if (value) {
      return 'Sí';
  } else if (value === false) {
      return 'No';
  } else {
      return 'N/A'
  }
}

export function getSubmissionStatus (value) {
  if (value) {
      return 'Aprobado';
  } else if (value === false) {
      return 'Rechazado';
  } else {
      return 'Pendiente'
  }
}

export function getReviewStatus (value1, value2, value3) {
  if (value1 === true && value2 !== null) {
    return 'Aprobado';
  } else if (value1 === false) {
    return 'Rechazado';
  } else if (value3 === true && value1 === null) {
    return 'Pendiente';
  } else {
    return 'N/A';
  }
}


export function appendDecimal (value) {
  if (value - Math.floor(value) === 0) {
    return `${value}.00`
  }
  else return `${value}`
}

  export function appendDollarSign (value) {
    if (value !== null) {
      return `$${value.toLocaleString()}`
    }
    else {
     return 'N/A'
    }
  }

  export function appendPercentageSign (value) {
    if (value !== null) {
      return `${value.toLocaleString()}%`
    }
    else {
      return 'N/A'
    }
  }

  export function appendPercentageSignAndDecimal (value) {
    if (value !== null && value - Math.floor(value) === 0) {
      return `${value.toLocaleString()}.00%` 
    }
    else   return `${value.toLocaleString()}%`

  }


// show decimal if value doesn't have any

export function showDecimal (value) {
  if (value === null ||
      value - Math.floor(value) === 0
  )
     return `$${value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
  else {
    return `$${value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
  }
}