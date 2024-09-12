import { Dispatch, SetStateAction } from "react";
import Contact from "../../models/Contact";

export const ContactTable = ({ contacts, onSelectRows } : { contacts?: Contact[]; onSelectRows: Dispatch<SetStateAction<number[]>> }) => {
  const handleChange = (ev: any ) => {
    const contactId = +ev.target.dataset.id;
    const isContactSelected = ev.target.checked;

    onSelectRows((prevRows) => {
      if (prevRows === undefined) {
        return [contactId];
      }

      const updatedRows = prevRows.slice();
      const indexToUpdate = prevRows.indexOf(contactId);

      if (indexToUpdate === -1) {
        return [...updatedRows, contactId].sort();
      }

      if (isContactSelected) {
        updatedRows[indexToUpdate] = contactId;
        return [...updatedRows].sort();
      }

      if (!isContactSelected) {
        const rowsAfterRemoving = [...updatedRows.filter(id => id !== contactId)].sort();
        return rowsAfterRemoving.length > 0 ? rowsAfterRemoving : [];
      }

      return [];
    });
  }

  return (
    <div className="overflow-x-auto">
      <table className="table">
        <thead>
          <tr>
            <th></th>
            <th>Nome</th>
            <th>DDD</th>
            <th>Telefone</th>
            <th>Email</th>
          </tr>
        </thead>
        <tbody>
          { ((contacts === undefined) || (contacts.length === 0))  && (
            <tr>
              <td colSpan={5} className="text-center">Não há contatos para exibição.</td>
            </tr>
          ) }
          { contacts !== undefined && contacts.length > 0 && contacts.map((contact) => (
            <tr key={contact.id}>
              <td><input type="checkbox" data-id={contact.id} className="checkbox" onChange={handleChange} /></td>
              <td>{contact.nome}</td>
              <td>{contact.ddd}</td>
              <td>{contact.telefone}</td>
              <td>{contact.email}</td>
            </tr>
          )) }
        </tbody>
      </table>
    </div>
  );
} 