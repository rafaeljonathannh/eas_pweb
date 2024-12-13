import java.io.IOException;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;

@WebServlet("/UpdateUserServlet")
public class UpdateUserServlet extends HttpServlet {
    private static final long serialVersionUID = 1L;

    protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
        // Check if the user is logged in
        HttpSession session = request.getSession(false);
        String loggedUser = (session != null) ? (String) session.getAttribute("loggedUser") : null;

        if (loggedUser == null) {
            response.getWriter().println("<p>You are not authorized to update this profile.</p>");
            return;
        }

        // Get the form parameters
        String name = request.getParameter("name");
        String password = request.getParameter("password");
        String email = request.getParameter("email");
        String dob = request.getParameter("dob");
        String address = request.getParameter("address");

        // Input validation (basic)
        if (name == null || name.trim().isEmpty() || email == null || email.trim().isEmpty()) {
            response.getWriter().println("<p>Name and email are required.</p>");
            return;
        }

        try (Connection conn = DriverManager.getConnection("jdbc:mysql://localhost:3306/storify", "root", "");
             PreparedStatement pstmt = conn.prepareStatement(
                "UPDATE users SET UserFullName=?, UserPassword=?, UserEmail=?, UserDOB=?, UserAddress=? WHERE UserName=?"
             )) {
            pstmt.setString(1, name);
            pstmt.setString(2, password);  // Note: Should hash this password before storing!
            pstmt.setString(3, email);
            pstmt.setString(4, dob);
            pstmt.setString(5, address);
            pstmt.setString(6, loggedUser);

            // Execute the update
            int updatedRows = pstmt.executeUpdate();
            if (updatedRows > 0) {
                response.sendRedirect("profile.jsp");  // Redirect to profile page
            } else {
                response.getWriter().println("<p>Failed to update profile. Try again later.</p>");
            }
        } catch (Exception e) {
            e.printStackTrace();
            response.getWriter().println("<p>Error updating profile. Please try again later.</p>");
        }
    }
}
