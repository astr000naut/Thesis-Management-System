const numberFormatter = {
  format: (s) => {
    if (s == null || s == "") return s;
    return s.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
  },
  getNumber: (s) => {
    if (s == null || s == "") return null;
    return parseInt(s.toString().replace(/\./g, ""));
  },
};

export default numberFormatter;
